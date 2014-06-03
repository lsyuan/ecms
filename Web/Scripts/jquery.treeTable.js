/*{
* jQuery treeTable Plugin 2.3.0
* http://ludo.cubicphuse.nl/jquery-plugins/treeTable/
* http://ludo.cubicphuse.nl/jquery-plugins/treeTable/doc/index.html#examples
*
* Copyright 2010, Ludo van den Boom
* Dual licensed under the MIT or GPL Version 2 licenses.
* 压缩不能给定 Shrink variables 
}*/

(function ($) {

	var defaultPaddingLeft;
	$.treeTable = {
		defaults: {
			clickableNodeNames: false,
			expandable: true,
			indent: 19,
			initialState: "collapsed",
			treeColumn: -1,
			dragDrop: { //拖动,可拖动对象不能是tr自身,可以是tr的直接或者间接子对象
				draggable: false, //是否启用节点拖动
				draggableClass: null, //可拖动节点的class,要以点号(.)开头,比如".draggable"
				droppableClass: null, //可接受拖入节点的class,要以点号(.)开头,比如".droppable"
				onDragDrop: null	//回调函数,原型：function (self,target),self,target 分别是一个tr,返回true表示拖动成功,false表示拖动失败
			},
			onSelected: null, 	// 选中一行之后原型：function (node)
			key: null,
			parentKey: null,
			addRemoveCallBack: null, // 对Tree(或List)的DOM作出了修改时调用
			//如果不指定ajax,那全部一次性绑定完成,不进行ajax处理
			//如果指定data,不指定ajax,那全部一次性绑定完成,不进行ajax处理
			//如果指定data,并指定ajax,那首先加载data,以后进行ajax处理
			//如果指定ajax,不指定data,那需要第一次进行ajax调用,如果指定ajaxLoad,那以后也需要进行ajax处理
			bind: {
				templateID: null,
				rowTemplate: null,
				data: null,
				rootParentKey: null,
				url: null,
				ajaxLoad: false, 	// 是否开启 ajax 加载数据,只针对树形,参数名会按照rootParentKey指定的值为参数名称，然后合并ajaxPara组织成后台调用参数
				ajaxPara: null, 	// ajax 调用时，附加参数
				ajaxSuccess: null, 	// ajax调用成功执行
				ajaxError: null, 	// ajax调用失败执行
				ajaxComplete: null, // ajax调用完成（不管成功失败）都执行
				firstLoaded: null, 	// 第一次绑定之后回调
				beforeBind: null	// 在对数据进行绑定前，对数据进行处理
			},
			paging: null//{ container:null, pageSize: 20, pageIndex: 0, url: null }
		}
	};

	$.fn.treeTable = function (opts) {
		var $this = $(this);
		$('tbody', $this).remove();
		$this.append('<tbody>');

		//OnSelected
		if (($this.data('options') == null || ($this.data('options') != null && $this.data('options').onSelected == null)) && (opts != null && opts.onSelected != null)) {
			$("tbody tr", $this).live("click", function () {
				var old = $(".row_selected", $(this).parent());
				var oldKey = old.length > 0 ? old.ttKey() : '';
				if ($(this).ttKey() != oldKey) {
					$(".row_selected", $(this).parent()).removeClass("row_selected");
					$(this).addClass("row_selected");
					if (options.onSelected != null) { options.onSelected($(this), $this.data("ExtData")); }
				}
			});
		}

		var options = $.extend({}, $.treeTable.defaults, opts);

		if (options.paging != null) {
			options.paging = jQuery.extend({
				pageSize: 20,
				pageIndex: 0
			}, options.paging || {});
		}
		$(this).data('options', options);

		if (options.bind.rowTemplate == null && options.bind.templateID != null) {
			var template = $(options.bind.templateID).clone();
			template.removeAttr("id"); //去除id属性，避免key="id"的情况下发生冲突
			template[0].style.display = '';
			options.bind.rowTemplate = $($('<p></p>').html(template)).html();
		}

		var template = $(options.bind.rowTemplate);
		if (options.key != null) { template.attr(options.key, '{' + options.key + '}'); }
		if (options.parentKey != null) { template.attr(options.parentKey, '{' + options.parentKey + '}'); }

		if (options.treeColumn >= 0) { $(template.children("td")[options.treeColumn]).addClass('treeColumn'); }
		if (options.paging != null) { options.paging.colspan = $('td', template).length; }

		options.bind.rowTemplate = '<!--data-->' + $('<p>').append(template).html() + '<!--data-->';

		//-----------LP ADD------------------------------------------
		if (!$this.hasClass("treeTable")) {

			//hover行样式<!--data-->
			$this.addClass("treeTable");
			//设置Table边框
			$this.attr("bordercolor", "d0d0d0");
			$this.attr("border", "1");

			$("tbody tr", $this).live("mouseover mouseout", function () { $(this).toggleClass("hover"); });
		}

		if (options.paging != null && options.paging.container != null) setPaging($this);
		else {
			//如果没有指定options.bind.data 并且指定了 获取数据的 AjaxUrl
			if (options.bind.data == null && options.bind.url != null) {
				loadAjaxData(
					options.bind.rootParentKey,
					options,
					function (data) {
						$('tbody', $this).append(bindData(data, options));
						firstInit($this, options);
						callAddRemoveCallBack(options);
					}
				);
			}
			else {
				$('tbody', $this).append(bindData(options.bind.data, options));
				firstInit($this, options);
				callAddRemoveCallBack(options);
			}
		}
		return $this;
	};

	// 公共方法{
	$.fn.ttOptions = function () { return $($(this).parents('table')[0]).data('options'); };

	// 增加节点数据,
	// 如果是TreeTable, this指向的是当前节点,
	// 如果是普通Table, this指向的是当前TreeTable,
	$.fn.ttAddNodes = function (data) {
		var node = $(this);
		var options = node.ttOptions();
		if (options == null) options = node.data('options');

		var addedNoes = $(bindData(data, options));
		var isTable = ($("tbody", node).length > 0);
		var rows = $("tbody tr", node);

		if (options.treeColumn < 0) {
			if (rows.length > 0) { $(rows[0]).before(addedNoes); }
			else { $('tbody', this).html(addedNoes); }
			SetAlternateRowClass($('tbody', this));
			//启用拖动
			dragDrop(addedNoes);
			callAddRemoveCallBack(options);
			return addedNoes;
		}

		if (isTable) { rows.last().after(addedNoes); }
		else { node.after(addedNoes); }

		if (addedNoes.length > 0) {
			dragDrop(addedNoes);

			var padding = defaultPaddingLeft;
			if (!isTable) {
				var cell = $(node.children("td")[options.treeColumn]);
				padding = getPaddingLeft(cell) + options.indent;
			}

			//必须手工重新初始化新增加子节点
			addedNoes.each(function () {
				if (!isTable) { $(this).children("td")[options.treeColumn].style.paddingLeft = padding + "px"; }
				initialize($(this));
			});
		}
		if (!isTable) {
			if (node.ttChildren().length == 0) { removeTreeFlag(node); }
			else if (!node.hasClass('parent')) {
				node.removeClass('initialized');
				initialize(node);
			}
			$(this).ttExpand();
		}
		callAddRemoveCallBack(options);
		return addedNoes;
	};

	$.fn.ttGetItem = function (key) { return $('tr[' + $(this).data('options').key + '="' + key + '"]', this); };
	$.fn.ttSelectedItem = function () { return $(".row_selected", this); };

	$.fn.ttSetSelectItem = function (key) {
		$(".row_selected", this).removeClass("row_selected");
		if (key == null) { return this; }
		var node = this.ttGetItem(key);
		if (node.length > 0) { node.addClass("row_selected"); }
		return node;
	};

	//如果调用对象是tree,那返回当前选择行的key属性
	//如果调用对象是tr,那返回当前tr的key属性
	$.fn.ttKey = function () {
		var tbody = $('tbody', this);
		if (tbody.length > 0) { return this.ttSelectedItem().attr(this.data('options').key); }

		return $(this).attr($(this).ttOptions().key);
	};

	//如果调用对象是tree,那返回当前选择行的parentKey属性
	//如果调用对象是tr,那返回当前tr的parentKey属性
	$.fn.ttParentKey = function () {
		var tbody = $('tbody', this);
		if (tbody.length > 0) { return this.ttSelectedItem().attr(this.data('options').parentKey); }

		return $(this).attr($(this).ttOptions().parentKey);
	};

	$.fn.ttModify = function (data) {
		var options = $(this).ttOptions();
		var node = $(bindData(data, options));

		var cell;
		if (options.treeColumn >= 0) { cell = $(this.children("td")[options.treeColumn]); }

		$(this).before(node);
		$(this).remove();

		//$(this).html(node.html());
		if (options.treeColumn >= 0) {
			node.removeClass('initialized');
			node.removeClass('parent');

			node.children("td")[options.treeColumn].style.paddingLeft = getPaddingLeft(cell) + "px";
			initialize(node);
		}
		//启用拖动
		dragDrop(node);
		callAddRemoveCallBack(options);
		return node;
	};

	$.fn.ttRemove = function () {
		var options = $(this).ttOptions();
		var tbody = $('tbody', this);

		if (tbody.length > 0) { tbody.html(''); }
		else {
			tbody = $(this).parent();
			if (options.treeColumn < 0) { $(this).remove(); }
			else {
				var parent = $(this).ttParent();
				removeNode($(this));

				if (parent != null && parent.ttChildren().length == 0) { removeTreeFlag(parent); }
			}
			SetAlternateRowClass(tbody);
		}
		callAddRemoveCallBack(options);
		return this;
	};

	$.fn.ttChildren = function () {
		var options = $(this).ttOptions();
		return $("tr[" + options.parentKey + "='" + $(this).attr(options.key) + "']", $(this).parent());
	};

	$.fn.ttParent = function () {
		var options = $(this).ttOptions();
		if (options != null) {
			var parent = $("tr[" + options.key + "='" + $(this).attr(options.parentKey) + "']", $(this).parent());
			if (parent.length > 0) { return parent; }
		}
		return null;
	};

	// Recursively hide all node's children in a tree
	$.fn.ttCollapse = function () {
		collapse($(this));
		var options = $(this).ttOptions();
		SetAlternateRowClass($(this).parent());
		callAddRemoveCallBack(options);
		return this;
	};

	// Recursively show all node's children in a tree
	$.fn.ttExpand = function () {
		expand($(this));
		var options = $(this).ttOptions();
		SetAlternateRowClass($(this).parent());
		callAddRemoveCallBack(options);
		return this;
	};

	// Reveal a node by expanding all ancestors
	//	$.fn.ttReveal = function () {
	//		$(ancestorsOf($(this)).ttReverse()).each(function () {
	//			initialize($(this));
	//			$(this).ttExpand().show();
	//		});

	//		return this;
	//	};

	// Add an entire branch to +destination+
	$.fn.ttAppendBranchTo = function (destination) {
		var node = $(this);
		var options = node.ttOptions();
		var parent = node.ttParent();

		var ancestorNames = $.map(ancestorsOf($(destination)), function (a) { return $(a).attr(options.key); });

		// Conditions:
		// 1: +node+ should not be inserted in a location in a branch if this would
		//    result in +node+ being an ancestor of itself.
		// 2: +node+ should not have a parent OR the destination should not be the
		//    same as +node+'s current parent (this last condition prevents +node+
		//    from being moved to the same location where it already is).
		// 3: +node+ should not be inserted as a child of +node+ itself.
		if ($.inArray(node.attr(options.key), ancestorNames) == -1 && (!parent || ($(destination).attr(options.key) != parent.attr(options.key)))
			&& $(destination).attr(options.key) != node.attr(options.key)) {

			indent(node, ancestorsOf(node).length * options.indent * -1); // Remove indentation

			node.attr(options.parentKey, $(destination).attr(options.key));
			move(node, destination); // Recursively move nodes to new location
			indent(node, ancestorsOf(node).length * options.indent);
		}

		return this;
	};

	// Add ttReverse() function from JS Arrays
	$.fn.ttReverse = function () { return this.pushStack(this.get().reverse(), arguments); };

	// Toggle an entire branch
	$.fn.ttToggleBranch = function () {
		var $this = $(this);
		if ($this.hasClass("collapsed")) {
			var options = $this.ttOptions();
			if (options.bind.url != null && options.bind.ajaxLoad == true && $this.ttChildren().length == 0) {
				loadAjaxData(
					$this.attr(options.key),
					options,
					function (data) { $this.ttAddNodes(data, options); }
				);
			}
			else { $this.ttExpand(); }
		}
		else { $this.removeClass("expanded").ttCollapse(); }

		return $this;
	};
	//}

	//移除节点前边的展开/收起标志（此节点为叶节点）
	$.fn.ttRemoveTreeFlag = function () {
		removeTreeFlag(this);
	}


	// 私有函数{
	function firstInit($this, options) {
		if (options.treeColumn < 0) {
			SetAlternateRowClass($("tbody", $this));
			return $this;
		}

		//DragDrop
		dragDrop($('tbody', $this));
		//-----------LP ADD END------------------------------------------

		$this.each(function () {
			var rootKey = null;
			$(this).find("tbody tr").each(function () {
				if (rootKey == null) rootKey = $(this).attr(options.parentKey);
				// Initialize root nodes only if possible
				if (!options.expandable || $(this).attr(options.parentKey) == rootKey) {
					// To optimize performance of indentation, I retrieve the padding-left
					// value of the first root node. This way I only have to call +css+ 
					// once.
					if (isNaN(defaultPaddingLeft)) { defaultPaddingLeft = parseInt($($(this).children("td")[options.treeColumn]).css('padding-left'), 10); }

					initialize($(this));
				}
				else if (options.initialState == "collapsed") { this.style.display = "none"; } // Performance! $(this).hide() is slow...
			});
		});
		SetAlternateRowClass($("tbody", $this));
	};

	function collapse(node) {
		node.addClass("collapsed");
		node.ttChildren().each(function () {
			if (!$(this).hasClass("collapsed")) { collapse($(this)); }
			this.style.display = "none"; // Performance! $(this).hide() is slow...
		});
	};

	function expand(node) {
		node.removeClass("collapsed").addClass("expanded");
		node.ttChildren().each(function () {
			initialize($(this));

			if ($(this).is(".expanded.parent")) { expand($(this)); }

			// this.style.display = "table-row"; // Unfortunately this is not possible with IE :-(
			this.style.display = ""; // Performance! $(this).show() is slow...
			//$(this).show();
		});
	};

	function setPaging($table) {
		var options = $table.data('options');
		$.ajax({
			url: options.paging.url,
			data: processAjaxPara(options.bind.ajaxPara),
			type: "POST",
			dataType: "json",
			cache: false,
			success: function (count) {
				$(options.paging.container).pagination(
					count,
					jQuery.extend({
						callback: function (pageIndex) {
							options.paging.pageIndex = pageIndex;
							loadAjaxData(
								options.bind.rootParentKey,
								options,
								function (data) { $('tbody', $table).html(bindData(data, options)); }
							);
							SetAlternateRowClass($('tbody', $table));
							return false;
						}
					}, options.paging)
				);
				if (options.bind.ajaxSuccess) { options.bind.ajaxSuccess(count); }
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (options.bind.ajaxError) { options.bind.ajaxError(XMLHttpRequest, textStatus, errorThrown); }
				else { ErrorInfo(XMLHttpRequest, textStatus, errorThrown); }
			},
			complete: options.bind.ajaxComplete
		});
	};

	function removeNode(node) {
		$(node).ttChildren().each(function () { removeNode(this); });
		$(node).remove();
	};

	function SetAlternateRowClass(tbody) {

		var options = $(tbody).ttOptions();
		$("tr:visible", tbody).removeClass("even").removeClass("odd");
		if (options.treeColumn < 0) {
			$("tr:odd", tbody).addClass("odd");
			$("tr:even", tbody).addClass("even");
			return;
		}

		var style = "even";
		$("tr:visible", tbody).each(function (idx, e) {
			$(e).addClass(style);
			style = style == "odd" ? "even" : "odd";
		});
	};

	function processAjaxPara(d) {
		for (var k in d) {
			if (typeof (d[k]) == "object" && typeof (d[k]) != null)
				d[k] = JSON.stringify(d[k]);
		}
		return d;
	};

	function loadAjaxData(parentID, options, callBack) {
		var pData = {};
		if (options.parentKey != null) { pData[options.parentKey] = parentID; }
		$.extend(pData, options.bind.ajaxPara, options.paging);

		$.ajax({
			url: options.bind.url,
			data: processAjaxPara(pData),
			type: "POST",
			dataType: "json",
			cache: false,
			success: function (data) {
				if (callBack != null) {
					callBack(data);
					callAddRemoveCallBack(options);
				}
				if (options.bind.ajaxSuccess) { options.bind.ajaxSuccess(data); }
				if (options.bind.firstLoaded) { options.bind.firstLoaded(); options.bind.firstLoaded = null; }
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (options.bind.ajaxError) { options.bind.ajaxError(XMLHttpRequest, textStatus, errorThrown); }
				else { ErrorInfo(XMLHttpRequest, textStatus, errorThrown); }
			},
			complete: options.bind.ajaxComplete
		});
	};

	function removeTreeFlag(node) {
		var options = $(node).ttOptions();
		$(node).removeClass("parent");
		var cell = $(node.children("td")[options.treeColumn]);
		if (cell.length > 0) { $(cell[0].firstChild).remove(); }
	};

	function bindData(data, options) {
		if (options.bind.beforeBind != null) options.bind.beforeBind(data);
		return $().bindTo(data, { template: options.bind.rowTemplate });
	};

	function dragDrop(selector) {

		var options = $(selector).ttOptions();
		if (options.dragDrop != null && options.dragDrop.draggable == true) {

			var dragSelector = options.dragDrop.draggableClass;
			if ($.isArray(options.dragDrop.draggableClass)) { dragSelector = options.dragDrop.draggableClass.join(','); }
			var dropSelector = options.dragDrop.droppableClass;
			if ($.isArray(options.dragDrop.droppableClass)) { dropSelector = options.dragDrop.droppableClass.join(','); }

			$(dragSelector, selector).draggable({
				helper: "clone",
				opacity: .75,
				refreshPositions: true,
				revert: "invalid",
				revertDuration: 300,
				scroll: true
			});

			$(dropSelector, selector).each(function () {
				$(this).parents("tr").droppable({
					drop: function (e, ui) {
						var dragRow = $(ui.draggable).parents("tr");
						var oldParent = dragRow.ttParent();

						if ((oldParent == null || (oldParent != null && dragRow.attr(options.key) != oldParent.attr(options.key)))
							&& options.dragDrop.onDragDrop != null) {
							if (options.dragDrop.onDragDrop(dragRow, $(this)) == false) { return false; }
						}

						$(this).ttToggleBranch();

						dragRow.ttAppendBranchTo(this);
						if (!$(this).hasClass("parent")) {
							//由于没有parent Class,那就没有绑定展开事件,可以重新进行初始化
							$(this).removeClass("initialized");
							initialize($(this));
						}

						if (oldParent != null && oldParent.ttChildren().length < 1) { removeTreeFlag($(oldParent)); }
					},
					hoverClass: "accept"
				});
			});
		}
	};

	function ancestorsOf(node) {
		var ancestors = [];
		while (node = node.ttParent()) { ancestors[ancestors.length] = node[0]; }
		return ancestors;
	};

	function getPaddingLeft(node) {
		var paddingLeft = parseInt(node[0].style.paddingLeft, 10);
		return (isNaN(paddingLeft)) ? defaultPaddingLeft : paddingLeft;
	};

	function indent(node, value) {
		var cell = $(node.children("td")[$(node).ttOptions().treeColumn]);
		cell[0].style.paddingLeft = getPaddingLeft(cell) + value + "px";

		node.ttChildren().each(function () { indent($(this), value); });
	};

	function initialize(node) {
		if (!node.hasClass("initialized")) {
			node.addClass("initialized");
			var options = $(node).ttOptions();

			var childNodes = node.ttChildren();

			if (!node.hasClass("parent") && (childNodes.length > 0 /*PYC*/ || (options.bind.url != null && options.bind.ajaxLoad == true /*PYC*/))) {
				node.addClass("parent");
			}

			if (node.hasClass("parent")) {
				var cell = $(node.children("td")[options.treeColumn]);
				var padding = getPaddingLeft(cell) + options.indent;

				childNodes.each(function () { $(this).children("td")[options.treeColumn].style.paddingLeft = padding + "px"; });

				if (options.expandable) {
					cell.prepend('<span style="margin-left: -' + options.indent + 'px; padding-left: ' + options.indent + 'px" class="expander"></span>');
					$(cell[0].firstChild).click(function () {
						node.ttToggleBranch();
						return false; //停止事件冒泡
					});

					if (options.clickableNodeNames) {
						cell[0].style.cursor = "pointer";
						$(cell).click(function (e) {
							// Don't double-toggle if the click is on the existing expander icon
							if (e.target.className != 'expander') { node.ttToggleBranch(); }
						});
					}

					// Check for a class set explicitly by the user, otherwise set the default class
					if (!(node.hasClass("expanded") || node.hasClass("collapsed"))) { node.addClass(options.initialState); }
					if (node.hasClass("expanded")) { node.ttExpand(); }
				}
			}
		}
	};

	function move(node, destination) {
		node.insertAfter(destination);
		node.ttChildren().ttReverse().each(function () { move($(this), node[0]); });
	};

	function callAddRemoveCallBack(options) {
		if (options.addRemoveCallBack != null) options.addRemoveCallBack();
	};
	//}

})(jQuery);
