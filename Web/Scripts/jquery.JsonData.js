
/*  需要前置 jquery 1.42+ 、jquery.validate.js。 */

//=================== $.fn.GroupVal ==========================================================================================================
/*
* 功能		:	获取/设置一组或单个 checkbox/radio 的value值。
*
* 使用方法	:	a.只给一个参数  ，在this范围内取name相同的一组或单个 checkbox/radio 的值(value);
*						如：$("#容器ID").GroupVal("group1");
*				b.给2个或3个参数，在this范围内设name相同的一组或单个 checkbox/radio 的值(value)。
*						如: $("#容器ID").GroupVal("group1",["1","2","男"],false);	
*							$("#容器ID").GroupVal("group1","1");		
*
* 函数原型	:	$("#容器ID").GroupVal(name,data,clearOld)
*	
* 参数		:	name:			checkbox/radio 的 name
*				data:			要设置的值，可为数组（设置多个checkbox）或单个值(设置单个checkbox、radio或多个radio)
*				clearOld:		设置值的时候，是否清除旧的值（设置为未选状态）, 【默认为true】
* 返回值		:
*				一、是多个 checkbox，返回值是数组或null(没有勾选任何一个)；
*				二、其他情况，单个checkbox、radio或者多个radio，返回单个值；
* 注意！		:
*				JQuery $("[value='']) 是不能取到value=''的对象的。因此要赋值的checkbox 或 radio的value不能设为""，要赋的值也不能为""
}*/
(function ($) {
	function GroupVal_Get(name, jqObjContainer) {
		
		//获得要取值的控件
		var items = jqObjContainer.find("[name='" + name + "']");
		if (items.length < 1) { return null; }
		
		// 只有一个 checkbox 或 radio
		if (items.length == 1) {
			return items.attr("checked") ? items.val() : null;
		}

		// 多个checkbox 或 radio
		var val = new Array();
		$(items).each(function () {
			if ($(this).attr("checked")) {
				val.push($(this).val());
			}
		});

		if ($(items[0]).attr("type").toLowerCase() == "radio") {
			// 多个 radio ， radio是单选，因此只返回单个值或null
			return (val.length > 0) ? val[0] : null;
		}
		else {
			// 多个 checkbox
			return (val.length > 0) ? val : null;
		}
	}

	function GroupVal_Put(name, data, jqObjContainer, clearOld) {
		//获得要设置的控件
		var items = $($.format("[name='{0}']", name), jqObjContainer);
		if (items.length < 1) { return null; }
		//如果clearOld != false，控件都设为未选状态
		if (clearOld != false) {
			items.each(function () { $(this).attr("checked", false); });
		}
		//根据data设置控件
		if (data == null) {
			return;
		}
		var ary = (new Array()).concat(data);
		$(ary).each(function (i, n) {
			if (n == null) {
				return;
			}
			var obj = $($.format("[name='{0}'][value='{1}']", name, n), jqObjContainer);
			if (obj.length > 0) {
				obj.attr("checked", true);
			}
		});
	}

	$.fn.GroupVal = function (name, data, clearOld) {
		var gvT = this;
		if (arguments.length > 1) {
			//********************赋值*************************
			this.each(function (index, domEle) {
				GroupVal_Put(name, data, gvT, clearOld);
			});
		}
		else {
			//********************取值*************************
			if (this.length > 1) {
				var re = new Array();
				for (var i = 0; i < this.length; i++) {
					re.push(GroupVal_Get(name, $(this[i])));
				}
				return re;
			}
			else {
				return GroupVal_Get(name, gvT);
			}
		}
	};
	
})(jQuery);

//=================== $.fn.JsonData ==========================================================================================================
/*{ 
* 功能		：		取指定容器内的数据，组织成为Json对象, 或者将Json对象数据绑定到指定的容器
* 使用方法	：		a.无参数, 取指定容器内的数据，组织成为 Json对象;
*						如：$("#divContainer").JsonData()
*					b.1-2个参数, 将 Json对象数据 绑定到指定的容器.
*						如：$("#divContainer").JsonData({"Sex":"男", "Age":25.5,"HomeAddr":"四川成都","Height":178},false);
*
* 参数		：		data:要绑定的Json数据
*					clearOld:是否清除旧的数据,默认为true
*
* 扩展html标签属性：
*	1、dn ：		意指 data name ，作为Json对象的属性名称；
*	2、get:		提供自定义【获取】数据的接口，由调用者提供复杂数据获取的方法 , 如:get="GetXmlAddr", 注意不要带括号。
*				方法的实现：
*							function GetXmlAddr(jqObj){         //jqObj，设置了dn的对象
*								 ......
*								 return ?;
*							}
*						
*	3、set:		提供自定义【绑定】数据的接口，由调用者提供复杂数据的绑定方法 , 如: set="SetAddr", 注意不要带括号。
*				方法的实现：
*							function SetAddr(data,jqObj){		//data 是dn对应的数据, jqObj是设置了dn的对象
*								......
*							} 
*
*	4、数据类型申明（支持: 字符串、整型数、浮点数、bool）
*		读取/绑定公共支持： 	
*			(1) dn	  默认是字符串类型
* 			(2) dn:n  整型数
*			(3) dn:f  浮点数
*			(4) dn:b  bool
*		绑定特殊支持：
*			(1) dn:dt{yyyy-MM-dd hh:mm:ss} 表示日期时间数据，并格式化成  {"格式"};
*				注：设置的数据支持.net服务器端序列化过来的格式：/Date(1284825600000)/.	
*		    
*	 5.其他说明
*		(1) 处理了 checkbox(单/组) 和 radio (单/组)赋值，自动调用GroupVal获取/设置数据.
}*/
(function ($) {
	

	//$.fn.JsonData使用的工具函数 {
	/*---生成层级对象的值 -------------------------------------------------------------------------------
	* 例:
	*	......
	*	var A={};
	*	SetObjValue(A, "b.c.d", 123)
	*	......
	*	则：A.b.c.d = 123
	*/
	function SetObjValue(jsonObj, attributes, val) {
		var tmp = jsonObj;
		var y = attributes.split('.');

		for (var i = 0; i < y.length - 1; i++) {
			if (tmp[y[i]] == null) tmp[y[i]] = {};
			tmp = tmp[y[i]];
		}
		tmp[y[y.length - 1]] = val;
	}
	/*---获得层级对象的值 -------------------------------------------------------------------------------
	* 例:
	*	......
	*	var A={a:{b:123}};
	*	GetObjValue(A, "a.b") // = 123
	*	......
	*/
	function GetObjValue(jsonObj, attr) {
		if (jsonObj == null) {
			return null;
		}
		var tmp = jsonObj;
		var attrs = attr.split('.');
		for (var i = 0; i < attrs.length; i++) {
			if (tmp[attrs[i]] == null) {
				return null;
			}
			tmp = tmp[attrs[i]];
		}
		return tmp;
	}

	function JsonData_Get(jqObjContainer) {
		//表单对象标签Array
		var inputTags = ["input", "select", "textarea"];
		//获得dn标注的Dom对象数组
		var items = $("[dn]", jqObjContainer);
		if (items == null || items.length < 1) {
			return null;
		}
		//遍历取值
		var jsonData = {};
		items.each(function () {
			var val;
			//取得自定义取值函数
			var g = $(this).attr("get");
			//取得Dom对象的type
			var type = $(this).attr("type");
			//取得Dom对象的tagName
			var tag = this.tagName.toLowerCase();
			if (g == null) {
				//对 checkbox 或 radio组进行特殊处理取值
				/*
				checkbox:	只有1个checkbox，返回 一个值 或者 null
				多个checkbox, 返回值数组 或者 null
				radio：	返回一个值或null
				*/
				if (tag == "input" && (type == "checkbox" || type == "radio")) {
					val = jqObjContainer.GroupVal($(this).attr("name"));
					if (val == null) val = $(this).attr("checked") ? $(this).attr("value") : null;
				}
				else if ($.inArray(tag, inputTags) > -1) {
					//input,select,textarea 表单输入对象取值 
					val = $(this).val();
				}
				else {
					//其他Dom对象取 html 值
					val = $(this).html();
				}
			}
			else {
				//自定义get取值
				var userFunc = eval(g);
				val = userFunc($(this));
				//eval($.format("val = {0}();", g));
			}
			//处理取得的数据
			var n = $(this).attr("dn").split(":");
			if (n.length > 1) {
				//处理整型类型
				if (n[1] == "n") {
					val = parseInt(val);
					if (val.toString() == "NaN") {
						val = null;
					}
				}
				//处理浮点数类型
				else if (n[1] == "f") {
					val = parseFloat(val);
					if (val.toString() == "NaN") {
						val = null;
					}
				}
				//处理Boolean类型
				else if (n[1] == "b") {
					val = (val == null || val.toLowerCase() != "true") ? false : true;
				}
			}
			//else {}//其余是字符串类型无需处理，也无需escape编码

			//jsonData[n[0]] = val;

			var jd = jsonData;
			var attrs = n[0].split(".");
			var cnt = attrs.length - 1;
			for (var i = 0; i < cnt; i++) {
				if (jd.hasOwnProperty([attrs[i]])) {
					jd = jd[attrs[i]];
				}
				else {
					jd = jd[attrs[i]] = {};
				}
			}
			jd[attrs[cnt]] = val;
		});
		return jsonData;
	}

	function JsonData_Put(data, jqObjContainer, clearOld) {
		var inputTags = ["input", "select", "textarea"];
		//获得dn标注的Dom对象数组
		var items = $("[dn]", jqObjContainer);
		if (items == null || items.length < 1) {
			return null;
		}

		//遍历赋值
		items.each(function () {
			//取得自定义赋值函数
			var set = $(this).attr("set");
			//取dn标记信息	
			var dn = $(this).attr("dn").split(":");
			//取清理旧数据后，绑定的数据为空时，要设置成的值 . 默认为""
			var clear = $(this).attr("clear");
			clear = (clear == null) ? "" : clear;
		    //取 data name
			var d = dn[0];
			// 获得欲赋的值 ,比如 data.XmlAddr.PostAddress.One
			var setData = GetObjValue(data, d);
            //处理动态查询语句返回的属性全部为大写的情况add by yyq 2013-04-09
            if(setData==undefined || setData==null){
                setData=GetObjValue(data,d.toUpperCase());
            }

			//数据类型处理
			if (dn.length > 1) {
				var di = dn[1];
                if (di =='dt{yyyy-MM-dd}' ) {
                    setData = setData.toString().substr(0,setData.toString().indexOf(' '));
                }
				//#region 日期类型数据处理开始，
				// 解析 /Date(1284825600000)/ 成为 指定的格式，默认：{yyyy-MM-dd hh:mm:ss}
//				var reg1 = /^(dt)/ig;
//				var reg2 = /{.*}$/ig;
//				if (reg1.test(di)) {
//					var fmt = "{yyyy-MM-dd hh:mm:ss}";
//					if (reg2.test(di)) {
//						fmt = di.match(reg2)[0];
//					}
//					var reg3 = /(Date\(\d+\))/ig;
//					var reg4 = /(Date\(-\d+\))/ig;
//					if (reg4.test(setData)) {
//						setData = "";
//					}
//					else if (reg3.test(setData)) {
//						setData = FormatDate(setData.match(reg3), fmt);
//					}
//				}
				//#endregion 日期类型数据处理结束
			}

			//取得Dom对象的tagName
			var tag = this.tagName.toLowerCase();
			
			//取得Dom对象的type
			var type = $(this).attr("type");

			//自定义赋值
			if (set != null) {
				var f = eval(set);
				f(setData, $(this));
			}
			//绑定数据
			else if (setData != null) {
				//对 checkbox 或 radio 组进行赋值
				if (tag == "input" && (type == "checkbox" || type == "radio")) {
					jqObjContainer.GroupVal($(this).attr("name"), setData, clearOld);
				}
				//表单输入对象val(value)赋值
				else if ($.inArray(tag, inputTags) > -1) {
					$(this).val(setData);
				}
				//其他对象html(value)赋值
				else {
					$(this).html(setData);
				}
			}
			//绑定的数据为空时，如果 clearOld!=false ，则清理Dom对象的旧数据
			else if (clearOld != false) {
				if (tag == "input" && (type == "checkbox" || type == "radio")) {
					jqObjContainer.GroupVal($(this).attr("name"), null, true);
				}
				else {
					if ($.inArray(tag, inputTags) > -1) {
						$(this).val(clear);
					}
					//其他对象html(value)赋值
					else {
						$(this).html(clear);
					}
				}
			}
		});

	}
	//}
	$.fn.JsonData = function (data, clearOld) {
		if (arguments.length > 0) {
			//********************赋值*************************
			this.each(function (index, domEle) {
				JsonData_Put(data, $(this), clearOld);
			});
		}
		else {
			//********************取值*************************
			if (this.length > 1) {
				var re = new Array();
				for (var i = 0; i < this.length; i++) {
					re.push(JsonData_Get($(this[i])));
				}
				return re;
			}
			else {
				return JsonData_Get(this);
			}
		}
	};
})(jQuery);

//#region ===================Tools Functions ===================================================================================

/*格式化日期成字符串，可指定格式。
*   
*   参数：
*       data 可以是 "/Date(1284825600000)/"类型的字符串，或者是一个Date
*       fmt默认为 "yyyy-MM-dd hh:mm:ss"， yyyy(年) MM(月) dd(日) hh(时) mm(分) ss(秒)
*
*   返回值：
*       格式化好的字符串。
*/
function FormatDate(date, fmt) {
	var d = date;
	if (!d) return "";
	if ((Object.prototype.toString.call(d).match(/object\s(\w+)/)[1]).toLowerCase() != "date") {
		d = d.toString();
		var reg = /(Date\(\d+\))/ig;
		eval($.format("d = new {0};", d.match(reg)));
	}
	fmt = (fmt == null) ? "yyyy-MM-dd hh:mm:ss" : fmt;
	return fmt.replace("yyyy", PadStr(d.getYear(), 4, "0"))
		      .replace("MM", PadStr(d.getMonth() + 1, 2, "0"))
		      .replace("dd", PadStr(d.getDate(), 2, "0"))
		      .replace("hh", PadStr(d.getHours(), 2, "0"))
		      .replace("mm", PadStr(d.getMinutes(), 2, "0"))
		      .replace("ss", PadStr(d.getSeconds(), 2, "0"))
		      .replace("{", "")
		      .replace("}", "");
}

//填充字符串
function PadStr(str, len, pad, onRight) {
	str = str.toString();
	if (str.length >= len) { return str; }
	var n = Math.floor((len - str.length) / pad.length);
	var p = (len - str.length) % pad.length;
	if (onRight) {
		return str + (new Array(n + 1).join(pad)) + pad.substring(0, p);
	}
	else {
		return (new Array(n + 1).join(pad)) + pad.substring(0, p) + str;
	}
}

//#endregion
 