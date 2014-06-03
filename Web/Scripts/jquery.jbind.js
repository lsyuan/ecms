(function ($) {
	/*
	* jBind 1.5.8
	*
	* Copyright (c) 2009 Arash Karimzadeh (arashkarimzadeh.com)
	* Licensed under the MIT (MIT-LICENSE.txt)
	* http://www.opensource.org/licenses/mit-license.php
	*
	* Date: Mar 07 2009
	*/
	$.fn.bindTo = function (data, options) {
		var defaults = { appendTo: null, root: 'data', onBind: null, onBound: null, fill: false, template: null };
		var options = $.extend(defaults, options);

		if ($.isFunction(options.onBind)) { options.onBind(); }

		/*PYC*/
		//解决当前jquery对象有多个元素的情况
		//增加直接设置绑定模板字符串的功能
		var innerHtml = options.template;
		if (innerHtml == null) {
			innerHtml = this.html();
			if (innerHtml == null) {
				innerHtml = "";
				//必须指定Clone,因为需要保留原来的DOM
				this.each(function () { innerHtml += $($('<p></p>').html($(this).clone())).html(); });
			}
		}

		var template = '<!--bind.template-->' + innerHtml.replace(/%7B/g, '{').replace(/%7D/g, '}') + '<!--bind.template-->';
		//解决IE将属性的前后引号去掉的bug
		template = template.replace(/=({[^}]*})/ig, "='$1'");
		/*PYC*/
		var repeaters = this.bindTo.findRepeaters(template);
		var fixedData = {};
		fixedData[options.root] = data;
		var content = this.bindTo.traverse(
								'bind.template',
								fixedData,
								repeaters['<!--bind.template-->'].template,
								repeaters,
								'bind.template');
		if (options.fill) { this.html(content); }
		if (options.appendTo != null) { content = $(content).appendTo($(options.appendTo)); }
		if ($.isFunction(options.onBound)) { options.onBound(content, data); }
		return content;
	};
	$.extend(
		$.fn.bindTo,
		{
			templates: {},
			traverse: function (key, data, template, repeaters, parent) {
				if (typeof data == 'string' || typeof data == 'number' || typeof data == 'boolean' || data == null) {
					return template.replace(new RegExp("\{" + key + "\}", "ig"), data);
				}
				else if (typeof data == 'object') {
					if (typeof data.length == 'undefined') {
						if (repeaters['<!--' + parent + '-->'].action) {
							template = $.fn.bindTo[repeaters['<!--' + parent + '-->'].action](template, data) || template;
						}
						for (var item in data) {
							if (typeof data[item] == 'object' /*PYC*/ && data[item] != null/*PYC*/) {
								if (typeof repeaters['<!--' + item + '-->'] != 'undefined') {//Skip not defined templates for child in aggregate object
									var temp = $.fn.bindTo.traverse(item, data[item], repeaters['<!--' + item + '-->'].template, repeaters, item);
									template = template.replace('<!--' + item + '-->', temp);
								}
							}
							else {
								/*PYC*/if (data[item] == null) data[item] = ''; /*PYC*/
								var temp = $.fn.bindTo.traverse(item, data[item], template, repeaters);
								template = temp;
							}
						}
						return template;
					}
					else {
						var listTemplate = '';
						for (var item in data) {
							listTemplate += $.fn.bindTo.traverse(item, data[item], repeaters['<!--' + key + '-->'].template, repeaters, key);
						}
						return listTemplate;
					}
				}
				return ''; //(handle extjs) this will return '' for function in case you extned array or object
			},
			findRepeaters: function (template) {
				$this = this;
				var templates = {};
				var reg = new RegExp('<!--[.a-zA-Z1-9]*-->', 'g');
				var regAction = new RegExp('<!--action:[$.a-zA-Z1-9]*-->', 'g');
				var matches = (template.match(reg));
				$.each(
					matches,
					function () {
						if (templates[this] != undefined)/*template is already added because end tag and start tags are the same*/
							return true;
						templates[this] = {};
						var temp = template.substring(template.indexOf(this) + this.length, template.lastIndexOf(this));
						var innerMatches = (temp.match(reg)) || [];
						$.each(
							innerMatches,
							function () {
								if (temp.indexOf(this) > -1) {
									var innerRepeater = temp.substring(temp.indexOf(this), temp.lastIndexOf(this) + this.length);
									temp = temp.replace(innerRepeater, this);
								}
							}
						);
						var actions = (temp.match(regAction)) || [];
						var key = this;
						$.each(
							actions,
							function () {
								var action = this.substring(11, this.length - 3);
								templates[key].action = action;
								temp = temp.replace(actions, '');
							});
						templates[this].template = temp;
					}
				);
				return templates;
			}
		}
	);
	//
})(jQuery);