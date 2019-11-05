/*! modernizr 3.3.1 (Custom Build) | MIT *
 * http://modernizr.com/download/?-bgsizecover-flexbox-objectfit-setclasses !*/
!function(e,n,t){function r(e,n){return typeof e===n}function o(){var e,n,t,o,i,s,a;for(var f in C)if(C.hasOwnProperty(f)){if(e=[],n=C[f],n.name&&(e.push(n.name.toLowerCase()),n.options&&n.options.aliases&&n.options.aliases.length))for(t=0;t<n.options.aliases.length;t++)e.push(n.options.aliases[t].toLowerCase());for(o=r(n.fn,"function")?n.fn():n.fn,i=0;i<e.length;i++)s=e[i],a=s.split("."),1===a.length?Modernizr[a[0]]=o:(!Modernizr[a[0]]||Modernizr[a[0]]instanceof Boolean||(Modernizr[a[0]]=new Boolean(Modernizr[a[0]])),Modernizr[a[0]][a[1]]=o),g.push((o?"":"no-")+a.join("-"))}}function i(e){var n=_.className,t=Modernizr._config.classPrefix||"";if(w&&(n=n.baseVal),Modernizr._config.enableJSClass){var r=new RegExp("(^|\\s)"+t+"no-js(\\s|$)");n=n.replace(r,"$1"+t+"js$2")}Modernizr._config.enableClasses&&(n+=" "+t+e.join(" "+t),w?_.className.baseVal=n:_.className=n)}function s(e){return e.replace(/([a-z])-([a-z])/g,function(e,n,t){return n+t.toUpperCase()}).replace(/^-/,"")}function a(e,n){return!!~(""+e).indexOf(n)}function f(){return"function"!=typeof n.createElement?n.createElement(arguments[0]):w?n.createElementNS.call(n,"http://www.w3.org/2000/svg",arguments[0]):n.createElement.apply(n,arguments)}function l(e,n){return function(){return e.apply(n,arguments)}}function u(e,n,t){var o;for(var i in e)if(e[i]in n)return t===!1?e[i]:(o=n[e[i]],r(o,"function")?l(o,t||n):o);return!1}function d(e){return e.replace(/([A-Z])/g,function(e,n){return"-"+n.toLowerCase()}).replace(/^ms-/,"-ms-")}function p(){var e=n.body;return e||(e=f(w?"svg":"body"),e.fake=!0),e}function c(e,t,r,o){var i,s,a,l,u="modernizr",d=f("div"),c=p();if(parseInt(r,10))for(;r--;)a=f("div"),a.id=o?o[r]:u+(r+1),d.appendChild(a);return i=f("style"),i.type="text/css",i.id="s"+u,(c.fake?c:d).appendChild(i),c.appendChild(d),i.styleSheet?i.styleSheet.cssText=e:i.appendChild(n.createTextNode(e)),d.id=u,c.fake&&(c.style.background="",c.style.overflow="hidden",l=_.style.overflow,_.style.overflow="hidden",_.appendChild(c)),s=t(d,e),c.fake?(c.parentNode.removeChild(c),_.style.overflow=l,_.offsetHeight):d.parentNode.removeChild(d),!!s}function m(n,r){var o=n.length;if("CSS"in e&&"supports"in e.CSS){for(;o--;)if(e.CSS.supports(d(n[o]),r))return!0;return!1}if("CSSSupportsRule"in e){for(var i=[];o--;)i.push("("+d(n[o])+":"+r+")");return i=i.join(" or "),c("@supports ("+i+") { #modernizr { position: absolute; } }",function(e){return"absolute"==getComputedStyle(e,null).position})}return t}function v(e,n,o,i){function l(){d&&(delete P.style,delete P.modElem)}if(i=r(i,"undefined")?!1:i,!r(o,"undefined")){var u=m(e,o);if(!r(u,"undefined"))return u}for(var d,p,c,v,h,y=["modernizr","tspan"];!P.style;)d=!0,P.modElem=f(y.shift()),P.style=P.modElem.style;for(c=e.length,p=0;c>p;p++)if(v=e[p],h=P.style[v],a(v,"-")&&(v=s(v)),P.style[v]!==t){if(i||r(o,"undefined"))return l(),"pfx"==n?v:!0;try{P.style[v]=o}catch(g){}if(P.style[v]!=h)return l(),"pfx"==n?v:!0}return l(),!1}function h(e,n,t,o,i){var s=e.charAt(0).toUpperCase()+e.slice(1),a=(e+" "+b.join(s+" ")+s).split(" ");return r(n,"string")||r(n,"undefined")?v(a,n,o,i):(a=(e+" "+E.join(s+" ")+s).split(" "),u(a,n,t))}function y(e,n,r){return h(e,t,t,n,r)}var g=[],C=[],x={_version:"3.3.1",_config:{classPrefix:"",enableClasses:!0,enableJSClass:!0,usePrefixes:!0},_q:[],on:function(e,n){var t=this;setTimeout(function(){n(t[e])},0)},addTest:function(e,n,t){C.push({name:e,fn:n,options:t})},addAsyncTest:function(e){C.push({name:null,fn:e})}},Modernizr=function(){};Modernizr.prototype=x,Modernizr=new Modernizr;var _=n.documentElement,w="svg"===_.nodeName.toLowerCase(),S="Moz O ms Webkit",b=x._config.usePrefixes?S.split(" "):[];x._cssomPrefixes=b;var z=function(n){var r,o=prefixes.length,i=e.CSSRule;if("undefined"==typeof i)return t;if(!n)return!1;if(n=n.replace(/^@/,""),r=n.replace(/-/g,"_").toUpperCase()+"_RULE",r in i)return"@"+n;for(var s=0;o>s;s++){var a=prefixes[s],f=a.toUpperCase()+"_"+r;if(f in i)return"@-"+a.toLowerCase()+"-"+n}return!1};x.atRule=z;var E=x._config.usePrefixes?S.toLowerCase().split(" "):[];x._domPrefixes=E;var j={elem:f("modernizr")};Modernizr._q.push(function(){delete j.elem});var P={style:j.elem.style};Modernizr._q.unshift(function(){delete P.style}),x.testAllProps=h,x.testAllProps=y,Modernizr.addTest("bgsizecover",y("backgroundSize","cover")),Modernizr.addTest("flexbox",y("flexBasis","1px",!0));var T=x.prefixed=function(e,n,t){return 0===e.indexOf("@")?z(e):(-1!=e.indexOf("-")&&(e=s(e)),n?h(e,n,t):h(e,"pfx"))};Modernizr.addTest("objectfit",!!T("objectFit"),{aliases:["object-fit"]}),o(),i(g),delete x.addTest,delete x.addAsyncTest;for(var N=0;N<Modernizr._q.length;N++)Modernizr._q[N]();e.Modernizr=Modernizr}(window,document);

/**
 * textFit v2.1.1
 * Previously known as jQuery.textFit
 * 11/2014 by STRML (strml.github.com)
 * MIT License
 *
 * To use: textFit(document.getElementById('target-div'), options);
 *
 * Will make the *text* content inside a container scale to fit the container
 * The container is required to have a set width and height
 * Uses binary search to fit text with minimal layout calls.
 * Version 2.0 does not use jQuery.
 */
/*global define:true, document:true, window:true, HTMLElement:true*/

(function(root, factory) {
  "use strict";

  // UMD shim
  if (typeof define === "function" && define.amd) {
    // AMD
    define([], factory);
  } else if (typeof exports === "object") {
    // Node/CommonJS
    module.exports = factory();
  } else {
    // Browser
    root.textFit = factory();
  }

}(typeof global === "object" ? global : this, function () {
  "use strict";

  var defaultSettings = {
    alignVert: false, // if true, textFit will align vertically using css tables
    alignHoriz: false, // if true, textFit will set text-align: center
    multiLine: false, // if true, textFit will not set white-space: no-wrap
    detectMultiLine: true, // disable to turn off automatic multi-line sensing
    minFontSize: 6,
    maxFontSize: 80,
    reProcess: true, // if true, textFit will re-process already-fit nodes. Set to 'false' for better performance
    widthOnly: false // if true, textFit will fit text to element width, regardless of text height
  };

  return function textFit(els, options, callback) {

    if (!options) options = {};

    if (typeof callback !== 'function') callback = function() {};

    // Extend options.
    var settings = {};
    for(var key in defaultSettings){
      if(options.hasOwnProperty(key)){
        settings[key] = options[key];
      } else {
        settings[key] = defaultSettings[key];
      }
    }

    // Convert jQuery objects into arrays
    if (typeof els.toArray === "function") {
      els = els.toArray();
    }

    // Support passing a single el
    var elType = Object.prototype.toString.call(els);
    if (elType !== '[object Array]' && elType !== '[object NodeList]'){
      els = [els];
    }

    // Process each el we've passed.
    for(var i = 0; i < els.length; i++){
      processItem(els[i], settings);
    }
  };

  /**
   * The meat. Given an el, make the text inside it fit its parent.
   * @param  {DOMElement} el       Child el.
   * @param  {Object} settings     Options for fit.
   */
  function processItem(el, settings){
    if (!isElement(el) || (!settings.reProcess && el.getAttribute('textFitted'))) {
      return false;
    }

    // Set textFitted attribute so we know this was processed.
    if(!settings.reProcess){
      el.setAttribute('textFitted', 1);
    }

    var innerSpan, originalHeight, originalHTML, originalWidth;
    var low, mid, high;

    // Get element data.
    originalHTML = el.innerHTML;
    originalWidth = innerWidth(el);
    originalHeight = innerHeight(el);

    // Don't process if we can't find box dimensions
    if (!originalWidth || (!settings.widthOnly && !originalHeight)) {
      if(!settings.widthOnly)
        throw new Error('Set a static height and width on the target element ' + el.outerHTML +
          ' before using textFit!');
      else
        throw new Error('Set a static width on the target element ' + el.outerHTML +
          ' before using textFit!');
    }

    // Add textFitted span inside this container.
    if (originalHTML.indexOf('textFitted') === -1) {
      innerSpan = document.createElement('span');
      innerSpan.className = 'textFitted';
      // Inline block ensure it takes on the size of its contents, even if they are enclosed
      // in other tags like <p>
      innerSpan.style['display'] = 'inline-block';
      innerSpan.innerHTML = originalHTML;
      el.innerHTML = '';
      el.appendChild(innerSpan);
    } else {
      // Reprocessing.
      innerSpan = el.querySelector('span.textFitted');
      // Remove vertical align if we're reprocessing.
      if (hasClass(innerSpan, 'textFitAlignVert')){
        innerSpan.className = innerSpan.className.replace('textFitAlignVert', '');
        innerSpan.style['height'] = '';
      }
    }

    // Prepare & set alignment
    if (settings.alignHoriz) {
      el.style['text-align'] = 'center';
      innerSpan.style['text-align'] = 'center';
    }

    // Check if this string is multiple lines
    // Not guaranteed to always work if you use wonky line-heights
    var multiLine = settings.multiLine;
    if (settings.detectMultiLine && !multiLine &&
        innerSpan.scrollHeight >= parseInt(window.getComputedStyle(innerSpan)['font-size'], 10) * 2){
      multiLine = true;
    }

    // If we're not treating this as a multiline string, don't let it wrap.
    if (!multiLine) {
      el.style['white-space'] = 'nowrap';
    }

    low = settings.minFontSize + 1;
    high = settings.maxFontSize + 1;

    // Binary search for best fit
    while (low <= high) {
      mid = parseInt((low + high) / 2, 10);
      innerSpan.style.fontSize = mid + 'px';
      if(innerSpan.scrollWidth <= originalWidth && (settings.widthOnly || innerSpan.scrollHeight <= originalHeight)){
        low = mid + 1;
      } else {
        high = mid - 1;
      }
    }
    // Sub 1 at the very end, this is closer to what we wanted.
    innerSpan.style.fontSize = (mid - 1) + 'px';

    // Our height is finalized. If we are aligning vertically, set that up.
    if (settings.alignVert) {
      addStyleSheet();
      var height = innerSpan.scrollHeight;
      if (window.getComputedStyle(el)['position'] === "static"){
        el.style['position'] = 'relative';
      }
      if (!hasClass(innerSpan, "textFitAlignVert")){
        innerSpan.className = innerSpan.className + " textFitAlignVert";
      }
      innerSpan.style['height'] = height + "px";
    }
  }

  // Calculate height without padding.
  function innerHeight(el){
    var style = window.getComputedStyle(el, null);
    return el.clientHeight -
      parseInt(style.getPropertyValue('padding-top'), 10) -
      parseInt(style.getPropertyValue('padding-bottom'), 10);
  }

  // Calculate width without padding.
  function innerWidth(el){
    var style = window.getComputedStyle(el, null);
    return el.clientWidth -
      parseInt(style.getPropertyValue('padding-left'), 10) -
      parseInt(style.getPropertyValue('padding-right'), 10);
  }

  //Returns true if it is a DOM element
  function isElement(o){
    return (
      typeof HTMLElement === "object" ? o instanceof HTMLElement : //DOM2
      o && typeof o === "object" && o !== null && o.nodeType === 1 && typeof o.nodeName==="string"
    );
  }

  function hasClass(element, cls) {
    return (' ' + element.className + ' ').indexOf(' ' + cls + ' ') > -1;
  }

  // Better than a stylesheet dependency
  function addStyleSheet() {
    if (document.getElementById("textFitStyleSheet")) return;
    var style = [
      ".textFitAlignVert{",
        "position: absolute;",
        "top: 0; right: 0; bottom: 0; left: 0;",
        "margin: auto;",
      "}"].join("");

    var css = document.createElement("style");
    css.type = "text/css";
    css.id = "textFitStyleSheet";
    css.innerHTML = style;
    document.body.appendChild(css);
  }
}));

/*! nouislider - 8.0.2 - 2015-07-06 13:22:09 */

!function(a){if("function"==typeof define&&define.amd)define([],a);else if("object"==typeof exports){var b=require("fs");module.exports=a(),module.exports.css=function(){return b.readFileSync(__dirname+"/nouislider.min.css","utf8")}}else window.noUiSlider=a()}(function(){"use strict";function a(a){return a.filter(function(a){return this[a]?!1:this[a]=!0},{})}function b(a,b){return Math.round(a/b)*b}function c(a){var b=a.getBoundingClientRect(),c=a.ownerDocument,d=c.defaultView||c.parentWindow,e=c.documentElement,f=d.pageXOffset;return/webkit.*Chrome.*Mobile/i.test(navigator.userAgent)&&(f=0),{top:b.top+d.pageYOffset-e.clientTop,left:b.left+f-e.clientLeft}}function d(a){return"number"==typeof a&&!isNaN(a)&&isFinite(a)}function e(a){var b=Math.pow(10,7);return Number((Math.round(a*b)/b).toFixed(7))}function f(a,b,c){j(a,b),setTimeout(function(){k(a,b)},c)}function g(a){return Math.max(Math.min(a,100),0)}function h(a){return Array.isArray(a)?a:[a]}function i(a){var b=a.split(".");return b.length>1?b[1].length:0}function j(a,b){a.classList?a.classList.add(b):a.className+=" "+b}function k(a,b){a.classList?a.classList.remove(b):a.className=a.className.replace(new RegExp("(^|\\b)"+b.split(" ").join("|")+"(\\b|$)","gi")," ")}function l(a,b){a.classList?a.classList.contains(b):new RegExp("(^| )"+b+"( |$)","gi").test(a.className)}function m(a,b){return 100/(b-a)}function n(a,b){return 100*b/(a[1]-a[0])}function o(a,b){return n(a,a[0]<0?b+Math.abs(a[0]):b-a[0])}function p(a,b){return b*(a[1]-a[0])/100+a[0]}function q(a,b){for(var c=1;a>=b[c];)c+=1;return c}function r(a,b,c){if(c>=a.slice(-1)[0])return 100;var d,e,f,g,h=q(c,a);return d=a[h-1],e=a[h],f=b[h-1],g=b[h],f+o([d,e],c)/m(f,g)}function s(a,b,c){if(c>=100)return a.slice(-1)[0];var d,e,f,g,h=q(c,b);return d=a[h-1],e=a[h],f=b[h-1],g=b[h],p([d,e],(c-f)*m(f,g))}function t(a,c,d,e){if(100===e)return e;var f,g,h=q(e,a);return d?(f=a[h-1],g=a[h],e-f>(g-f)/2?g:f):c[h-1]?a[h-1]+b(e-a[h-1],c[h-1]):e}function u(a,b,c){var e;if("number"==typeof b&&(b=[b]),"[object Array]"!==Object.prototype.toString.call(b))throw new Error("noUiSlider: 'range' contains invalid value.");if(e="min"===a?0:"max"===a?100:parseFloat(a),!d(e)||!d(b[0]))throw new Error("noUiSlider: 'range' value isn't numeric.");c.xPct.push(e),c.xVal.push(b[0]),e?c.xSteps.push(isNaN(b[1])?!1:b[1]):isNaN(b[1])||(c.xSteps[0]=b[1])}function v(a,b,c){return b?void(c.xSteps[a]=n([c.xVal[a],c.xVal[a+1]],b)/m(c.xPct[a],c.xPct[a+1])):!0}function w(a,b,c,d){this.xPct=[],this.xVal=[],this.xSteps=[d||!1],this.xNumSteps=[!1],this.snap=b,this.direction=c;var e,f=[];for(e in a)a.hasOwnProperty(e)&&f.push([a[e],e]);for(f.sort(function(a,b){return a[0]-b[0]}),e=0;e<f.length;e++)u(f[e][1],f[e][0],this);for(this.xNumSteps=this.xSteps.slice(0),e=0;e<this.xNumSteps.length;e++)v(e,this.xNumSteps[e],this)}function x(a,b){if(!d(b))throw new Error("noUiSlider: 'step' is not numeric.");a.singleStep=b}function y(a,b){if("object"!=typeof b||Array.isArray(b))throw new Error("noUiSlider: 'range' is not an object.");if(void 0===b.min||void 0===b.max)throw new Error("noUiSlider: Missing 'min' or 'max' in 'range'.");a.spectrum=new w(b,a.snap,a.dir,a.singleStep)}function z(a,b){if(b=h(b),!Array.isArray(b)||!b.length||b.length>2)throw new Error("noUiSlider: 'start' option is incorrect.");a.handles=b.length,a.start=b}function A(a,b){if(a.snap=b,"boolean"!=typeof b)throw new Error("noUiSlider: 'snap' option must be a boolean.")}function B(a,b){if(a.animate=b,"boolean"!=typeof b)throw new Error("noUiSlider: 'animate' option must be a boolean.")}function C(a,b){if("lower"===b&&1===a.handles)a.connect=1;else if("upper"===b&&1===a.handles)a.connect=2;else if(b===!0&&2===a.handles)a.connect=3;else{if(b!==!1)throw new Error("noUiSlider: 'connect' option doesn't match handle count.");a.connect=0}}function D(a,b){switch(b){case"horizontal":a.ort=0;break;case"vertical":a.ort=1;break;default:throw new Error("noUiSlider: 'orientation' option is invalid.")}}function E(a,b){if(!d(b))throw new Error("noUiSlider: 'margin' option must be numeric.");if(a.margin=a.spectrum.getMargin(b),!a.margin)throw new Error("noUiSlider: 'margin' option is only supported on linear sliders.")}function F(a,b){if(!d(b))throw new Error("noUiSlider: 'limit' option must be numeric.");if(a.limit=a.spectrum.getMargin(b),!a.limit)throw new Error("noUiSlider: 'limit' option is only supported on linear sliders.")}function G(a,b){switch(b){case"ltr":a.dir=0;break;case"rtl":a.dir=1,a.connect=[0,2,1,3][a.connect];break;default:throw new Error("noUiSlider: 'direction' option was not recognized.")}}function H(a,b){if("string"!=typeof b)throw new Error("noUiSlider: 'behaviour' must be a string containing options.");var c=b.indexOf("tap")>=0,d=b.indexOf("drag")>=0,e=b.indexOf("fixed")>=0,f=b.indexOf("snap")>=0;a.events={tap:c||f,drag:d,fixed:e,snap:f}}function I(a,b){if(a.format=b,"function"==typeof b.to&&"function"==typeof b.from)return!0;throw new Error("noUiSlider: 'format' requires 'to' and 'from' methods.")}function J(a){var b,c={margin:0,limit:0,animate:!0,format:U};b={step:{r:!1,t:x},start:{r:!0,t:z},connect:{r:!0,t:C},direction:{r:!0,t:G},snap:{r:!1,t:A},animate:{r:!1,t:B},range:{r:!0,t:y},orientation:{r:!1,t:D},margin:{r:!1,t:E},limit:{r:!1,t:F},behaviour:{r:!0,t:H},format:{r:!1,t:I}};var d={connect:!1,direction:"ltr",behaviour:"tap",orientation:"horizontal"};return Object.keys(d).forEach(function(b){void 0===a[b]&&(a[b]=d[b])}),Object.keys(b).forEach(function(d){var e=b[d];if(void 0===a[d]){if(e.r)throw new Error("noUiSlider: '"+d+"' is required.");return!0}e.t(c,a[d])}),c.pips=a.pips,c.style=c.ort?"top":"left",c}function K(a,b,c){var d=a+b[0],e=a+b[1];return c?(0>d&&(e+=Math.abs(d)),e>100&&(d-=e-100),[g(d),g(e)]):[d,e]}function L(a){a.preventDefault();var b,c,d=0===a.type.indexOf("touch"),e=0===a.type.indexOf("mouse"),f=0===a.type.indexOf("pointer"),g=a;return 0===a.type.indexOf("MSPointer")&&(f=!0),d&&(b=a.changedTouches[0].pageX,c=a.changedTouches[0].pageY),(e||f)&&(b=a.clientX+window.pageXOffset,c=a.clientY+window.pageYOffset),g.points=[b,c],g.cursor=e||f,g}function M(a,b){var c=document.createElement("div"),d=document.createElement("div"),e=["-lower","-upper"];return a&&e.reverse(),j(d,T[3]),j(d,T[3]+e[b]),j(c,T[2]),c.appendChild(d),c}function N(a,b,c){switch(a){case 1:j(b,T[7]),j(c[0],T[6]);break;case 3:j(c[1],T[6]);case 2:j(c[0],T[7]);case 0:j(b,T[6])}}function O(a,b,c){var d,e=[];for(d=0;a>d;d+=1)e.push(c.appendChild(M(b,d)));return e}function P(a,b,c){j(c,T[0]),j(c,T[8+a]),j(c,T[4+b]);var d=document.createElement("div");return j(d,T[1]),c.appendChild(d),d}function Q(b,d){function e(a,b,c){if("range"===a||"steps"===a)return M.xVal;if("count"===a){var d,e=100/(b-1),f=0;for(b=[];(d=f++*e)<=100;)b.push(d);a="positions"}return"positions"===a?b.map(function(a){return M.fromStepping(c?M.getStep(a):a)}):"values"===a?c?b.map(function(a){return M.fromStepping(M.getStep(M.toStepping(a)))}):b:void 0}function m(b,c,d){var e=M.direction,f={},g=M.xVal[0],h=M.xVal[M.xVal.length-1],i=!1,j=!1,k=0;return M.direction=0,d=a(d.slice().sort(function(a,b){return a-b})),d[0]!==g&&(d.unshift(g),i=!0),d[d.length-1]!==h&&(d.push(h),j=!0),d.forEach(function(a,e){var g,h,l,m,n,o,p,q,r,s,t=a,u=d[e+1];if("steps"===c&&(g=M.xNumSteps[e]),g||(g=u-t),t!==!1&&void 0!==u)for(h=t;u>=h;h+=g){for(m=M.toStepping(h),n=m-k,q=n/b,r=Math.round(q),s=n/r,l=1;r>=l;l+=1)o=k+l*s,f[o.toFixed(5)]=["x",0];p=d.indexOf(h)>-1?1:"steps"===c?2:0,!e&&i&&(p=0),h===u&&j||(f[m.toFixed(5)]=[h,p]),k=m}}),M.direction=e,f}function n(a,b,c){function e(a){return["-normal","-large","-sub"][a]}function f(a,b,c){return'class="'+b+" "+b+"-"+h+" "+b+e(c[1])+'" style="'+d.style+": "+a+'%"'}function g(a,d){M.direction&&(a=100-a),d[1]=d[1]&&b?b(d[0],d[1]):d[1],i.innerHTML+="<div "+f(a,"noUi-marker",d)+"></div>",d[1]&&(i.innerHTML+="<div "+f(a,"noUi-value",d)+">"+c.to(d[0])+"</div>")}var h=["horizontal","vertical"][d.ort],i=document.createElement("div");return j(i,"noUi-pips"),j(i,"noUi-pips-"+h),Object.keys(a).forEach(function(b){g(b,a[b])}),i}function o(a){var b=a.mode,c=a.density||1,d=a.filter||!1,f=a.values||!1,g=a.stepped||!1,h=e(b,f,g),i=m(c,b,h),j=a.format||{to:Math.round};return I.appendChild(n(i,d,j))}function p(){return G["offset"+["Width","Height"][d.ort]]}function q(a,b){void 0!==b&&(b=Math.abs(b-d.dir)),Object.keys(R).forEach(function(c){var d=c.split(".")[0];a===d&&R[c].forEach(function(a){a(h(B()),b,r(Array.prototype.slice.call(Q)))})})}function r(a){return 1===a.length?a[0]:d.dir?a.reverse():a}function s(a,b,c,e){var f=function(b){return I.hasAttribute("disabled")?!1:l(I,T[14])?!1:(b=L(b),a===S.start&&void 0!==b.buttons&&b.buttons>1?!1:(b.calcPoint=b.points[d.ort],void c(b,e)))},g=[];return a.split(" ").forEach(function(a){b.addEventListener(a,f,!1),g.push([a,f])}),g}function t(a,b){var c,d,e=b.handles||H,f=!1,g=100*(a.calcPoint-b.start)/p(),h=e[0]===H[0]?0:1;if(c=K(g,b.positions,e.length>1),f=y(e[0],c[h],1===e.length),e.length>1){if(f=y(e[1],c[h?0:1],!1)||f)for(d=0;d<b.handles.length;d++)q("slide",d)}else f&&q("slide",h)}function u(a,b){var c=G.getElementsByClassName(T[15]),d=b.handles[0]===H[0]?0:1;c.length&&k(c[0],T[15]),a.cursor&&(document.body.style.cursor="",document.body.removeEventListener("selectstart",document.body.noUiListener));var e=document.documentElement;e.noUiListeners.forEach(function(a){e.removeEventListener(a[0],a[1])}),k(I,T[12]),q("set",d),q("change",d)}function v(a,b){var c=document.documentElement;if(1===b.handles.length&&(j(b.handles[0].children[0],T[15]),b.handles[0].hasAttribute("disabled")))return!1;a.stopPropagation();var d=s(S.move,c,t,{start:a.calcPoint,handles:b.handles,positions:[J[0],J[H.length-1]]}),e=s(S.end,c,u,{handles:b.handles});if(c.noUiListeners=d.concat(e),a.cursor){document.body.style.cursor=getComputedStyle(a.target).cursor,H.length>1&&j(I,T[12]);var f=function(){return!1};document.body.noUiListener=f,document.body.addEventListener("selectstart",f,!1)}}function w(a){var b,e,g=a.calcPoint,h=0;return a.stopPropagation(),H.forEach(function(a){h+=c(a)[d.style]}),b=h/2>g||1===H.length?0:1,g-=c(G)[d.style],e=100*g/p(),d.events.snap||f(I,T[14],300),H[b].hasAttribute("disabled")?!1:(y(H[b],e),q("slide",b),q("set",b),q("change",b),void(d.events.snap&&v(a,{handles:[H[h]]})))}function x(a){var b,c;if(!a.fixed)for(b=0;b<H.length;b+=1)s(S.start,H[b].children[0],v,{handles:[H[b]]});a.tap&&s(S.start,G,w,{handles:H}),a.drag&&(c=[G.getElementsByClassName(T[7])[0]],j(c[0],T[10]),a.fixed&&c.push(H[c[0]===H[0]?1:0].children[0]),c.forEach(function(a){s(S.start,a,v,{handles:H})}))}function y(a,b,c){var e=a!==H[0]?1:0,f=J[0]+d.margin,h=J[1]-d.margin,i=J[0]+d.limit,l=J[1]-d.limit;return H.length>1&&(b=e?Math.max(b,f):Math.min(b,h)),c!==!1&&d.limit&&H.length>1&&(b=e?Math.min(b,i):Math.max(b,l)),b=M.getStep(b),b=g(parseFloat(b.toFixed(7))),b===J[e]?!1:(a.style[d.style]=b+"%",a.previousSibling||(k(a,T[17]),b>50&&j(a,T[17])),J[e]=b,Q[e]=M.fromStepping(b),q("update",e),!0)}function z(a,b){var c,e,f;for(d.limit&&(a+=1),c=0;a>c;c+=1)e=c%2,f=b[e],null!==f&&f!==!1&&("number"==typeof f&&(f=String(f)),f=d.format.from(f),(f===!1||isNaN(f)||y(H[e],M.toStepping(f),c===3-d.dir)===!1)&&q("update",e))}function A(a){var b,c,e=h(a);for(d.dir&&d.handles>1&&e.reverse(),d.animate&&-1!==J[0]&&f(I,T[14],300),b=H.length>1?3:1,1===e.length&&(b=1),z(b,e),c=0;c<H.length;c++)q("set",c)}function B(){var a,b=[];for(a=0;a<d.handles;a+=1)b[a]=d.format.to(Q[a]);return r(b)}function C(){T.forEach(function(a){a&&k(I,a)}),I.innerHTML="",delete I.noUiSlider}function D(){var a=J.map(function(a,b){var c=M.getApplicableStep(a),d=i(String(c[2])),e=Q[b],f=100===a?null:c[2],g=Number((e-c[2]).toFixed(d)),h=0===a?null:g>=c[1]?c[2]:c[0]||!1;return[h,f]});return r(a)}function E(a,b){R[a]=R[a]||[],R[a].push(b),"update"===a.split(".")[0]&&H.forEach(function(a,b){q("update",b)})}function F(a){var b=a.split(".")[0],c=a.substring(b.length);Object.keys(R).forEach(function(a){var d=a.split(".")[0],e=a.substring(d.length);b&&b!==d||c&&c!==e||delete R[a]})}var G,H,I=b,J=[-1,-1],M=d.spectrum,Q=[],R={};if(I.noUiSlider)throw new Error("Slider was already initialized.");return G=P(d.dir,d.ort,I),H=O(d.handles,d.dir,G),N(d.connect,I,H),x(d.events),d.pips&&o(d.pips),{destroy:C,steps:D,on:E,off:F,get:B,set:A}}function R(a,b){if(!a.nodeName)throw new Error("noUiSlider.create requires a single element.");var c=J(b,a),d=Q(a,c);d.set(c.start),a.noUiSlider=d}var S=window.navigator.pointerEnabled?{start:"pointerdown",move:"pointermove",end:"pointerup"}:window.navigator.msPointerEnabled?{start:"MSPointerDown",move:"MSPointerMove",end:"MSPointerUp"}:{start:"mousedown touchstart",move:"mousemove touchmove",end:"mouseup touchend"},T=["noUi-target","noUi-base","noUi-origin","noUi-handle","noUi-horizontal","noUi-vertical","noUi-background","noUi-connect","noUi-ltr","noUi-rtl","noUi-dragable","","noUi-state-drag","","noUi-state-tap","noUi-active","","noUi-stacking"];w.prototype.getMargin=function(a){return 2===this.xPct.length?n(this.xVal,a):!1},w.prototype.toStepping=function(a){return a=r(this.xVal,this.xPct,a),this.direction&&(a=100-a),a},w.prototype.fromStepping=function(a){return this.direction&&(a=100-a),e(s(this.xVal,this.xPct,a))},w.prototype.getStep=function(a){return this.direction&&(a=100-a),a=t(this.xPct,this.xSteps,this.snap,a),this.direction&&(a=100-a),a},w.prototype.getApplicableStep=function(a){var b=q(a,this.xPct),c=100===a?2:1;return[this.xNumSteps[b-2],this.xVal[b-c],this.xNumSteps[b-c]]},w.prototype.convert=function(a){return this.getStep(this.toStepping(a))};var U={to:function(a){return a.toFixed(2)},from:Number};return{create:R}});
function fiveForms(params){var params;if(null==params.id)return!1;var _this=this;this.form="string"==typeof params.id&&null!=document.getElementById(params.id)?document.getElementById(params.id):params.id,this.onError=null!=params.onError&&"function"==typeof params.onError?params.onError:function(){},this.onSuccess=null!=params.onSuccess&&"function"==typeof params.onSuccess?params.onSuccess:null,this.jQuery=null!=params.jQuery&&"boolean"==typeof params.jQuery?params.jQuery:!1,this.onFocus=null!=params.onFocus&&"function"==typeof params.onFocus?params.onFocus:function(){},this.onBlur=null!=params.onBlur&&"function"==typeof params.onBlur?params.onBlur:function(){},this.onClick=null!=params.onClick&&"function"==typeof params.onClick?params.onClick:function(){},this.errors=[],this.fields=[],this.customValidators=[],this.notify="number"==typeof params.notify||"string"==typeof params.notify||"boolean"==typeof params.notify?params.notify:"highlight",this.start=function(){for(var types=["input","textarea","select"],x=0;x<types.length;x++)for(var els=this.form.getElementsByTagName(types[x]),y=0;y<els.length;y++)if("true"==this.attr(els[y],"data-required")){var ty=this.attr(els[y],"data-validation-type"),ta=els[y].nodeName.toLowerCase();this.bind(els[y],"focus",_this.onFocusPreconfigured),this.bind(els[y],"focus",_this.onFocus),this.bind(els[y],"blur",_this.onBlur),this.bind(els[y],"click",_this.onClick),"true"==this.attr(els[y],"data-realtime")&&this.bind(els[y],"keyup",_this.onKeyUp),this.fields.push({obj:els[y],name:els[y].getAttribute("name"),type:0==ty?"input"==ta?"text"!=els[y].type?els[y].type:"text":ta:ty,tag:ta})}this.bind(this.form,"submit",this.process)},this.process=function(e){_this.preventDefaults(e);for(var results=[],x=0;x<_this.fields.length;x++)results[x]=_this.validateField(_this.fields[x]);_this.errors=[];for(var x=0;x<results.length;x++){var res=results[x],obj=_this.getFieldByName(res.obj.name);res.error&&_this.errors.push({name:res.obj.name,value:res.obj.value,type:res.type,tag:res.tag,obj:obj})}_this.processErrors(),_this.processFinish()},this.processErrors=function(){_this.onError(_this.errors),0!=_this.notify&&"false"!=_this.notify&&_this.notifications(_this.errors)},this.processFinish=function(){_this.errors.length<=0&&("function"==typeof _this.onSuccess?_this.onSuccess():"function"==typeof _this.form.submit?_this.form.submit():_this.log("Form could not be submitted."))},this.validateField=function(field){field.obj;field.error=!1,field.error=""==field.obj.value?!0:!1;var custom=_this.hasCustomValidator(field.type);if("function"==typeof custom)field.error=custom(field.obj.value);else switch(field.type){default:case"text":case"textarea":field.error=_this.validateTextOnly(field.obj.value);break;case"number":field.error=_this.validateNumberOnly(field.obj.value);break;case"email":field.error=_this.validateEmail(field.obj.value);break;case"address":field.error=_this.validateAddress(field.obj.value);break;case"province":field.error=_this.validProvince(field.obj.value);break;case"state":field.error=_this.validState(field.obj.value);break;case"postal":case"zip":field.error=_this.validPostal(field.obj.value);break;case"tel":field.error=_this.validatePhone(field.obj.value);break;case"dob":field.error=0==_this.dateFromString(field.obj.value,_this.attr(field.obj,"data-validation-dob-type"));break;case"select":field.error=0==field.obj.value||""==field.obj.value;break;case"checkbox":field.error=!field.obj.checked}return field},this.notifications=function(err){for(var err,msg=[],x=0;x<_this.fields.length;x++){for(var field=_this.fields[x],errored=!1,y=0;y<err.length;y++)errored=err[y].name==field.name?!0:errored;switch(_this.notify){default:case"highlight":_this.removeHighlight(field.obj),errored&&_this.addHighlight(field.obj);break;case"alert":errored&&"true"!=_this.attr(field.obj,"data-realtime")&&msg.push("Error: "+field.name+".")}}"alert"==_this.notify&&msg.length>=1&&alert(msg.join(" "))},this.addHighlight=function(field){var field;_this.addClass(field,"fiveForms-error");var label=field.parentNode.getElementsByTagName("label");0!=label.length&&_this.addClass(label[0],"fiveForms-error")},this.removeHighlight=function(field){var field;_this.removeClass(field,"fiveForms-error");var label=field.parentNode.getElementsByTagName("label");0!=label.length&&_this.removeClass(label[0],"fiveForms-error")},this.onFocusPreconfigured=function(){"highlight"==_this.notify&&_this.removeHighlight(this)},this.onKeyUp=function(){if(_this.errors=[],this.value.length>3){var valid=_this.validateField(this);if(valid.error){var res=_this.getFieldByName(this.name);_this.errors=[{name:this.name,value:this.value,type:res.type,tag:res.tag,obj:res.obj}]}}_this.processErrors()},this.getFieldByName=function(name){for(var name,x=0;x<_this.fields.length;x++)if(name==_this.fields[x].name)return _this.fields[x];return!1},this.addCustomValidator=function(type,func){var type,func;return _this.hasCustomValidator(type)?!1:(_this.customValidators.push({type:type,"function":func}),!0)},this.hasCustomValidator=function(type){for(var type,x=0;x<_this.customValidators.length;x++)if(_this.customValidators[x].type==type)return _this.customValidators[x]["function"];return!1},this.validateNumberOnly=function(val){var val;return!new RegExp(/^[0-9]+$/).test(val)},this.validateTextOnly=function(val){var val;return!new RegExp(/^[A-Za-z\-',.`''´ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÑÒÓÔÕÖÙÚÛÜÝàáâãäåæœçèéêëìíîïñòóôõöýüûúùÿ\s]+$/).test(val)},this.validateEmail=function(val){var val;return!new RegExp(/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/).test(val)},this.validateAddress=function(val){var val;return!new RegExp(/^[A-Za-z0-9\-#',.`‘’´ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÑÒÔÓÕÖÙÚÛÜÝàáâãäåæœçèéêëìíîïñòóôõöýüûúùÿ\s]+$/).test(val)},this.validProvince=function(val){var val;return!new RegExp(/^(AB|BC|MB|NB|NL|NT|NS|NV|ON|PE|QC|SK|YT)/i).test(val)},this.validState=function(val){var val;return!new RegExp(/^(A[LKSZRAEP]|C[AOT]|D[EC]|F[LM]|G[ANU]|HI|I[ADLN]|K[SY]|LA|M[ADEHINOPST]|N[CDEHJMVY]|O[HKR]|P[ARW]|RI|S[CD]|T[NX]|UT|V[AIT]|W[AIVY])$/i).test(val)},this.validPostal=function(val){var val;return!new RegExp(/(^[ABCEGHJKLMNPRSTVXY]\d[ABCEGHJKLMNPRSTVWXYZ]( )?\d[ABCEGHJKLMNPRSTVWXYZ]\d$)|(^\d{5}(-\d{4})?$)/i).test(val)},this.validatePhone=function(val){var val;return!new RegExp(/^[(]?[1-9]{1}[0-9]{2}[)]?[\s-]?[1-9]{1}[0-9]{2}[\s-]?[0-9]{4}$/).test(val)},this.attr=function(obj,attr){return"object"==typeof obj&&"string"==typeof attr&&null!=obj.getAttribute(attr)?obj.getAttribute(attr):!1},this.bind=function(obj,evt,fnc,useCapture){return useCapture||(useCapture=!1),"object"!=typeof obj||"string"!=typeof evt||"function"!=typeof fnc?!1:void(obj.addEventListener?obj.addEventListener(evt,fnc,useCapture):obj["on"+evt]=fnc)},this.addClass=function(obj,cls){var obj,cls;if("object"==typeof obj){var classes=""!=obj.className?obj.className.split(" "):[];return classes.push(cls),obj.className=classes.join(classes.length>1?" ":""),!0}return!1},this.removeClass=function(obj,cls){var obj,cls;if("object"==typeof obj){for(var classes=""!=obj.className?obj.className.split(" "):[obj.className],removed=[],x=0;x<classes.length;x++)classes[x]!=cls&&removed.push(classes[x]);return obj.className=removed.join(removed.length>1?" ":""),!0}return!1},this.preventDefaults=function(e){var evt=e||window.event;evt.preventDefault?evt.preventDefault():(evt.returnValue=!1,evt.cancelBubble=!0)},this.log=function(msg){var msg;"function"==typeof console.log?console.log(msg):alert(msg)},this.dateFromString=function(str,format){var str,month,day,year,date,format="mdy"==format||"dmy"==format||"ymd"==format||"ydm"==format?format:"mdy",match=str.match(/^\d+([.\/-])\d+\1\d+$/),today=new Date;if(null==match||"object"!=typeof match||match.length<2)return!1;var split=match[0].split(match[1]),day=parseInt("mdy"==format||"ydm"==format?split[1]:"dmy"==format?split[0]:"ymd"==format?split[2]:0),month=parseInt("mdy"==format?split[0]:"dmy"==format||"ymd"==format?split[1]:"ydm"==format?split[2]:0),year=parseInt("mdy"==format||"dmy"==format?split[2]:"ymd"==format||"ydm"==format?split[0]:0);return date=new Date(parseInt(year),parseInt(month)-1,parseInt(day)),day>31||month>12||year<today.getFullYear()-120||year>today.getFullYear()-13||isNaN(date.getTime())?!1:date},this.start()}"undefined"!=typeof jQuery&&!function($){$.fn.fiveForms=function(obj){return"object"==typeof obj?obj.id=this.get(0):obj={id:this.get(0)},new fiveForms(obj)}}(jQuery);
//! moment.js
//! version : 2.11.2
//! authors : Tim Wood, Iskren Chernev, Moment.js contributors
//! license : MIT
//! momentjs.com
!function(a,b){"object"==typeof exports&&"undefined"!=typeof module?module.exports=b():"function"==typeof define&&define.amd?define(b):a.moment=b()}(this,function(){"use strict";function a(){return Uc.apply(null,arguments)}function b(a){Uc=a}function c(a){return"[object Array]"===Object.prototype.toString.call(a)}function d(a){return a instanceof Date||"[object Date]"===Object.prototype.toString.call(a)}function e(a,b){var c,d=[];for(c=0;c<a.length;++c)d.push(b(a[c],c));return d}function f(a,b){return Object.prototype.hasOwnProperty.call(a,b)}function g(a,b){for(var c in b)f(b,c)&&(a[c]=b[c]);return f(b,"toString")&&(a.toString=b.toString),f(b,"valueOf")&&(a.valueOf=b.valueOf),a}function h(a,b,c,d){return Da(a,b,c,d,!0).utc()}function i(){return{empty:!1,unusedTokens:[],unusedInput:[],overflow:-2,charsLeftOver:0,nullInput:!1,invalidMonth:null,invalidFormat:!1,userInvalidated:!1,iso:!1}}function j(a){return null==a._pf&&(a._pf=i()),a._pf}function k(a){if(null==a._isValid){var b=j(a);a._isValid=!(isNaN(a._d.getTime())||!(b.overflow<0)||b.empty||b.invalidMonth||b.invalidWeekday||b.nullInput||b.invalidFormat||b.userInvalidated),a._strict&&(a._isValid=a._isValid&&0===b.charsLeftOver&&0===b.unusedTokens.length&&void 0===b.bigHour)}return a._isValid}function l(a){var b=h(NaN);return null!=a?g(j(b),a):j(b).userInvalidated=!0,b}function m(a){return void 0===a}function n(a,b){var c,d,e;if(m(b._isAMomentObject)||(a._isAMomentObject=b._isAMomentObject),m(b._i)||(a._i=b._i),m(b._f)||(a._f=b._f),m(b._l)||(a._l=b._l),m(b._strict)||(a._strict=b._strict),m(b._tzm)||(a._tzm=b._tzm),m(b._isUTC)||(a._isUTC=b._isUTC),m(b._offset)||(a._offset=b._offset),m(b._pf)||(a._pf=j(b)),m(b._locale)||(a._locale=b._locale),Wc.length>0)for(c in Wc)d=Wc[c],e=b[d],m(e)||(a[d]=e);return a}function o(b){n(this,b),this._d=new Date(null!=b._d?b._d.getTime():NaN),Xc===!1&&(Xc=!0,a.updateOffset(this),Xc=!1)}function p(a){return a instanceof o||null!=a&&null!=a._isAMomentObject}function q(a){return 0>a?Math.ceil(a):Math.floor(a)}function r(a){var b=+a,c=0;return 0!==b&&isFinite(b)&&(c=q(b)),c}function s(a,b,c){var d,e=Math.min(a.length,b.length),f=Math.abs(a.length-b.length),g=0;for(d=0;e>d;d++)(c&&a[d]!==b[d]||!c&&r(a[d])!==r(b[d]))&&g++;return g+f}function t(){}function u(a){return a?a.toLowerCase().replace("_","-"):a}function v(a){for(var b,c,d,e,f=0;f<a.length;){for(e=u(a[f]).split("-"),b=e.length,c=u(a[f+1]),c=c?c.split("-"):null;b>0;){if(d=w(e.slice(0,b).join("-")))return d;if(c&&c.length>=b&&s(e,c,!0)>=b-1)break;b--}f++}return null}function w(a){var b=null;if(!Yc[a]&&"undefined"!=typeof module&&module&&module.exports)try{b=Vc._abbr,require("./locale/"+a),x(b)}catch(c){}return Yc[a]}function x(a,b){var c;return a&&(c=m(b)?z(a):y(a,b),c&&(Vc=c)),Vc._abbr}function y(a,b){return null!==b?(b.abbr=a,Yc[a]=Yc[a]||new t,Yc[a].set(b),x(a),Yc[a]):(delete Yc[a],null)}function z(a){var b;if(a&&a._locale&&a._locale._abbr&&(a=a._locale._abbr),!a)return Vc;if(!c(a)){if(b=w(a))return b;a=[a]}return v(a)}function A(a,b){var c=a.toLowerCase();Zc[c]=Zc[c+"s"]=Zc[b]=a}function B(a){return"string"==typeof a?Zc[a]||Zc[a.toLowerCase()]:void 0}function C(a){var b,c,d={};for(c in a)f(a,c)&&(b=B(c),b&&(d[b]=a[c]));return d}function D(a){return a instanceof Function||"[object Function]"===Object.prototype.toString.call(a)}function E(b,c){return function(d){return null!=d?(G(this,b,d),a.updateOffset(this,c),this):F(this,b)}}function F(a,b){return a.isValid()?a._d["get"+(a._isUTC?"UTC":"")+b]():NaN}function G(a,b,c){a.isValid()&&a._d["set"+(a._isUTC?"UTC":"")+b](c)}function H(a,b){var c;if("object"==typeof a)for(c in a)this.set(c,a[c]);else if(a=B(a),D(this[a]))return this[a](b);return this}function I(a,b,c){var d=""+Math.abs(a),e=b-d.length,f=a>=0;return(f?c?"+":"":"-")+Math.pow(10,Math.max(0,e)).toString().substr(1)+d}function J(a,b,c,d){var e=d;"string"==typeof d&&(e=function(){return this[d]()}),a&&(bd[a]=e),b&&(bd[b[0]]=function(){return I(e.apply(this,arguments),b[1],b[2])}),c&&(bd[c]=function(){return this.localeData().ordinal(e.apply(this,arguments),a)})}function K(a){return a.match(/\[[\s\S]/)?a.replace(/^\[|\]$/g,""):a.replace(/\\/g,"")}function L(a){var b,c,d=a.match($c);for(b=0,c=d.length;c>b;b++)bd[d[b]]?d[b]=bd[d[b]]:d[b]=K(d[b]);return function(e){var f="";for(b=0;c>b;b++)f+=d[b]instanceof Function?d[b].call(e,a):d[b];return f}}function M(a,b){return a.isValid()?(b=N(b,a.localeData()),ad[b]=ad[b]||L(b),ad[b](a)):a.localeData().invalidDate()}function N(a,b){function c(a){return b.longDateFormat(a)||a}var d=5;for(_c.lastIndex=0;d>=0&&_c.test(a);)a=a.replace(_c,c),_c.lastIndex=0,d-=1;return a}function O(a,b,c){td[a]=D(b)?b:function(a,d){return a&&c?c:b}}function P(a,b){return f(td,a)?td[a](b._strict,b._locale):new RegExp(Q(a))}function Q(a){return R(a.replace("\\","").replace(/\\(\[)|\\(\])|\[([^\]\[]*)\]|\\(.)/g,function(a,b,c,d,e){return b||c||d||e}))}function R(a){return a.replace(/[-\/\\^$*+?.()|[\]{}]/g,"\\$&")}function S(a,b){var c,d=b;for("string"==typeof a&&(a=[a]),"number"==typeof b&&(d=function(a,c){c[b]=r(a)}),c=0;c<a.length;c++)ud[a[c]]=d}function T(a,b){S(a,function(a,c,d,e){d._w=d._w||{},b(a,d._w,d,e)})}function U(a,b,c){null!=b&&f(ud,a)&&ud[a](b,c._a,c,a)}function V(a,b){return new Date(Date.UTC(a,b+1,0)).getUTCDate()}function W(a,b){return c(this._months)?this._months[a.month()]:this._months[Ed.test(b)?"format":"standalone"][a.month()]}function X(a,b){return c(this._monthsShort)?this._monthsShort[a.month()]:this._monthsShort[Ed.test(b)?"format":"standalone"][a.month()]}function Y(a,b,c){var d,e,f;for(this._monthsParse||(this._monthsParse=[],this._longMonthsParse=[],this._shortMonthsParse=[]),d=0;12>d;d++){if(e=h([2e3,d]),c&&!this._longMonthsParse[d]&&(this._longMonthsParse[d]=new RegExp("^"+this.months(e,"").replace(".","")+"$","i"),this._shortMonthsParse[d]=new RegExp("^"+this.monthsShort(e,"").replace(".","")+"$","i")),c||this._monthsParse[d]||(f="^"+this.months(e,"")+"|^"+this.monthsShort(e,""),this._monthsParse[d]=new RegExp(f.replace(".",""),"i")),c&&"MMMM"===b&&this._longMonthsParse[d].test(a))return d;if(c&&"MMM"===b&&this._shortMonthsParse[d].test(a))return d;if(!c&&this._monthsParse[d].test(a))return d}}function Z(a,b){var c;return a.isValid()?"string"==typeof b&&(b=a.localeData().monthsParse(b),"number"!=typeof b)?a:(c=Math.min(a.date(),V(a.year(),b)),a._d["set"+(a._isUTC?"UTC":"")+"Month"](b,c),a):a}function $(b){return null!=b?(Z(this,b),a.updateOffset(this,!0),this):F(this,"Month")}function _(){return V(this.year(),this.month())}function aa(a){return this._monthsParseExact?(f(this,"_monthsRegex")||ca.call(this),a?this._monthsShortStrictRegex:this._monthsShortRegex):this._monthsShortStrictRegex&&a?this._monthsShortStrictRegex:this._monthsShortRegex}function ba(a){return this._monthsParseExact?(f(this,"_monthsRegex")||ca.call(this),a?this._monthsStrictRegex:this._monthsRegex):this._monthsStrictRegex&&a?this._monthsStrictRegex:this._monthsRegex}function ca(){function a(a,b){return b.length-a.length}var b,c,d=[],e=[],f=[];for(b=0;12>b;b++)c=h([2e3,b]),d.push(this.monthsShort(c,"")),e.push(this.months(c,"")),f.push(this.months(c,"")),f.push(this.monthsShort(c,""));for(d.sort(a),e.sort(a),f.sort(a),b=0;12>b;b++)d[b]=R(d[b]),e[b]=R(e[b]),f[b]=R(f[b]);this._monthsRegex=new RegExp("^("+f.join("|")+")","i"),this._monthsShortRegex=this._monthsRegex,this._monthsStrictRegex=new RegExp("^("+e.join("|")+")$","i"),this._monthsShortStrictRegex=new RegExp("^("+d.join("|")+")$","i")}function da(a){var b,c=a._a;return c&&-2===j(a).overflow&&(b=c[wd]<0||c[wd]>11?wd:c[xd]<1||c[xd]>V(c[vd],c[wd])?xd:c[yd]<0||c[yd]>24||24===c[yd]&&(0!==c[zd]||0!==c[Ad]||0!==c[Bd])?yd:c[zd]<0||c[zd]>59?zd:c[Ad]<0||c[Ad]>59?Ad:c[Bd]<0||c[Bd]>999?Bd:-1,j(a)._overflowDayOfYear&&(vd>b||b>xd)&&(b=xd),j(a)._overflowWeeks&&-1===b&&(b=Cd),j(a)._overflowWeekday&&-1===b&&(b=Dd),j(a).overflow=b),a}function ea(b){a.suppressDeprecationWarnings===!1&&"undefined"!=typeof console&&console.warn&&console.warn("Deprecation warning: "+b)}function fa(a,b){var c=!0;return g(function(){return c&&(ea(a+"\nArguments: "+Array.prototype.slice.call(arguments).join(", ")+"\n"+(new Error).stack),c=!1),b.apply(this,arguments)},b)}function ga(a,b){Jd[a]||(ea(b),Jd[a]=!0)}function ha(a){var b,c,d,e,f,g,h=a._i,i=Kd.exec(h)||Ld.exec(h);if(i){for(j(a).iso=!0,b=0,c=Nd.length;c>b;b++)if(Nd[b][1].exec(i[1])){e=Nd[b][0],d=Nd[b][2]!==!1;break}if(null==e)return void(a._isValid=!1);if(i[3]){for(b=0,c=Od.length;c>b;b++)if(Od[b][1].exec(i[3])){f=(i[2]||" ")+Od[b][0];break}if(null==f)return void(a._isValid=!1)}if(!d&&null!=f)return void(a._isValid=!1);if(i[4]){if(!Md.exec(i[4]))return void(a._isValid=!1);g="Z"}a._f=e+(f||"")+(g||""),wa(a)}else a._isValid=!1}function ia(b){var c=Pd.exec(b._i);return null!==c?void(b._d=new Date(+c[1])):(ha(b),void(b._isValid===!1&&(delete b._isValid,a.createFromInputFallback(b))))}function ja(a,b,c,d,e,f,g){var h=new Date(a,b,c,d,e,f,g);return 100>a&&a>=0&&isFinite(h.getFullYear())&&h.setFullYear(a),h}function ka(a){var b=new Date(Date.UTC.apply(null,arguments));return 100>a&&a>=0&&isFinite(b.getUTCFullYear())&&b.setUTCFullYear(a),b}function la(a){return ma(a)?366:365}function ma(a){return a%4===0&&a%100!==0||a%400===0}function na(){return ma(this.year())}function oa(a,b,c){var d=7+b-c,e=(7+ka(a,0,d).getUTCDay()-b)%7;return-e+d-1}function pa(a,b,c,d,e){var f,g,h=(7+c-d)%7,i=oa(a,d,e),j=1+7*(b-1)+h+i;return 0>=j?(f=a-1,g=la(f)+j):j>la(a)?(f=a+1,g=j-la(a)):(f=a,g=j),{year:f,dayOfYear:g}}function qa(a,b,c){var d,e,f=oa(a.year(),b,c),g=Math.floor((a.dayOfYear()-f-1)/7)+1;return 1>g?(e=a.year()-1,d=g+ra(e,b,c)):g>ra(a.year(),b,c)?(d=g-ra(a.year(),b,c),e=a.year()+1):(e=a.year(),d=g),{week:d,year:e}}function ra(a,b,c){var d=oa(a,b,c),e=oa(a+1,b,c);return(la(a)-d+e)/7}function sa(a,b,c){return null!=a?a:null!=b?b:c}function ta(b){var c=new Date(a.now());return b._useUTC?[c.getUTCFullYear(),c.getUTCMonth(),c.getUTCDate()]:[c.getFullYear(),c.getMonth(),c.getDate()]}function ua(a){var b,c,d,e,f=[];if(!a._d){for(d=ta(a),a._w&&null==a._a[xd]&&null==a._a[wd]&&va(a),a._dayOfYear&&(e=sa(a._a[vd],d[vd]),a._dayOfYear>la(e)&&(j(a)._overflowDayOfYear=!0),c=ka(e,0,a._dayOfYear),a._a[wd]=c.getUTCMonth(),a._a[xd]=c.getUTCDate()),b=0;3>b&&null==a._a[b];++b)a._a[b]=f[b]=d[b];for(;7>b;b++)a._a[b]=f[b]=null==a._a[b]?2===b?1:0:a._a[b];24===a._a[yd]&&0===a._a[zd]&&0===a._a[Ad]&&0===a._a[Bd]&&(a._nextDay=!0,a._a[yd]=0),a._d=(a._useUTC?ka:ja).apply(null,f),null!=a._tzm&&a._d.setUTCMinutes(a._d.getUTCMinutes()-a._tzm),a._nextDay&&(a._a[yd]=24)}}function va(a){var b,c,d,e,f,g,h,i;b=a._w,null!=b.GG||null!=b.W||null!=b.E?(f=1,g=4,c=sa(b.GG,a._a[vd],qa(Ea(),1,4).year),d=sa(b.W,1),e=sa(b.E,1),(1>e||e>7)&&(i=!0)):(f=a._locale._week.dow,g=a._locale._week.doy,c=sa(b.gg,a._a[vd],qa(Ea(),f,g).year),d=sa(b.w,1),null!=b.d?(e=b.d,(0>e||e>6)&&(i=!0)):null!=b.e?(e=b.e+f,(b.e<0||b.e>6)&&(i=!0)):e=f),1>d||d>ra(c,f,g)?j(a)._overflowWeeks=!0:null!=i?j(a)._overflowWeekday=!0:(h=pa(c,d,e,f,g),a._a[vd]=h.year,a._dayOfYear=h.dayOfYear)}function wa(b){if(b._f===a.ISO_8601)return void ha(b);b._a=[],j(b).empty=!0;var c,d,e,f,g,h=""+b._i,i=h.length,k=0;for(e=N(b._f,b._locale).match($c)||[],c=0;c<e.length;c++)f=e[c],d=(h.match(P(f,b))||[])[0],d&&(g=h.substr(0,h.indexOf(d)),g.length>0&&j(b).unusedInput.push(g),h=h.slice(h.indexOf(d)+d.length),k+=d.length),bd[f]?(d?j(b).empty=!1:j(b).unusedTokens.push(f),U(f,d,b)):b._strict&&!d&&j(b).unusedTokens.push(f);j(b).charsLeftOver=i-k,h.length>0&&j(b).unusedInput.push(h),j(b).bigHour===!0&&b._a[yd]<=12&&b._a[yd]>0&&(j(b).bigHour=void 0),b._a[yd]=xa(b._locale,b._a[yd],b._meridiem),ua(b),da(b)}function xa(a,b,c){var d;return null==c?b:null!=a.meridiemHour?a.meridiemHour(b,c):null!=a.isPM?(d=a.isPM(c),d&&12>b&&(b+=12),d||12!==b||(b=0),b):b}function ya(a){var b,c,d,e,f;if(0===a._f.length)return j(a).invalidFormat=!0,void(a._d=new Date(NaN));for(e=0;e<a._f.length;e++)f=0,b=n({},a),null!=a._useUTC&&(b._useUTC=a._useUTC),b._f=a._f[e],wa(b),k(b)&&(f+=j(b).charsLeftOver,f+=10*j(b).unusedTokens.length,j(b).score=f,(null==d||d>f)&&(d=f,c=b));g(a,c||b)}function za(a){if(!a._d){var b=C(a._i);a._a=e([b.year,b.month,b.day||b.date,b.hour,b.minute,b.second,b.millisecond],function(a){return a&&parseInt(a,10)}),ua(a)}}function Aa(a){var b=new o(da(Ba(a)));return b._nextDay&&(b.add(1,"d"),b._nextDay=void 0),b}function Ba(a){var b=a._i,e=a._f;return a._locale=a._locale||z(a._l),null===b||void 0===e&&""===b?l({nullInput:!0}):("string"==typeof b&&(a._i=b=a._locale.preparse(b)),p(b)?new o(da(b)):(c(e)?ya(a):e?wa(a):d(b)?a._d=b:Ca(a),k(a)||(a._d=null),a))}function Ca(b){var f=b._i;void 0===f?b._d=new Date(a.now()):d(f)?b._d=new Date(+f):"string"==typeof f?ia(b):c(f)?(b._a=e(f.slice(0),function(a){return parseInt(a,10)}),ua(b)):"object"==typeof f?za(b):"number"==typeof f?b._d=new Date(f):a.createFromInputFallback(b)}function Da(a,b,c,d,e){var f={};return"boolean"==typeof c&&(d=c,c=void 0),f._isAMomentObject=!0,f._useUTC=f._isUTC=e,f._l=c,f._i=a,f._f=b,f._strict=d,Aa(f)}function Ea(a,b,c,d){return Da(a,b,c,d,!1)}function Fa(a,b){var d,e;if(1===b.length&&c(b[0])&&(b=b[0]),!b.length)return Ea();for(d=b[0],e=1;e<b.length;++e)(!b[e].isValid()||b[e][a](d))&&(d=b[e]);return d}function Ga(){var a=[].slice.call(arguments,0);return Fa("isBefore",a)}function Ha(){var a=[].slice.call(arguments,0);return Fa("isAfter",a)}function Ia(a){var b=C(a),c=b.year||0,d=b.quarter||0,e=b.month||0,f=b.week||0,g=b.day||0,h=b.hour||0,i=b.minute||0,j=b.second||0,k=b.millisecond||0;this._milliseconds=+k+1e3*j+6e4*i+36e5*h,this._days=+g+7*f,this._months=+e+3*d+12*c,this._data={},this._locale=z(),this._bubble()}function Ja(a){return a instanceof Ia}function Ka(a,b){J(a,0,0,function(){var a=this.utcOffset(),c="+";return 0>a&&(a=-a,c="-"),c+I(~~(a/60),2)+b+I(~~a%60,2)})}function La(a,b){var c=(b||"").match(a)||[],d=c[c.length-1]||[],e=(d+"").match(Ud)||["-",0,0],f=+(60*e[1])+r(e[2]);return"+"===e[0]?f:-f}function Ma(b,c){var e,f;return c._isUTC?(e=c.clone(),f=(p(b)||d(b)?+b:+Ea(b))-+e,e._d.setTime(+e._d+f),a.updateOffset(e,!1),e):Ea(b).local()}function Na(a){return 15*-Math.round(a._d.getTimezoneOffset()/15)}function Oa(b,c){var d,e=this._offset||0;return this.isValid()?null!=b?("string"==typeof b?b=La(qd,b):Math.abs(b)<16&&(b=60*b),!this._isUTC&&c&&(d=Na(this)),this._offset=b,this._isUTC=!0,null!=d&&this.add(d,"m"),e!==b&&(!c||this._changeInProgress?cb(this,Za(b-e,"m"),1,!1):this._changeInProgress||(this._changeInProgress=!0,a.updateOffset(this,!0),this._changeInProgress=null)),this):this._isUTC?e:Na(this):null!=b?this:NaN}function Pa(a,b){return null!=a?("string"!=typeof a&&(a=-a),this.utcOffset(a,b),this):-this.utcOffset()}function Qa(a){return this.utcOffset(0,a)}function Ra(a){return this._isUTC&&(this.utcOffset(0,a),this._isUTC=!1,a&&this.subtract(Na(this),"m")),this}function Sa(){return this._tzm?this.utcOffset(this._tzm):"string"==typeof this._i&&this.utcOffset(La(pd,this._i)),this}function Ta(a){return this.isValid()?(a=a?Ea(a).utcOffset():0,(this.utcOffset()-a)%60===0):!1}function Ua(){return this.utcOffset()>this.clone().month(0).utcOffset()||this.utcOffset()>this.clone().month(5).utcOffset()}function Va(){if(!m(this._isDSTShifted))return this._isDSTShifted;var a={};if(n(a,this),a=Ba(a),a._a){var b=a._isUTC?h(a._a):Ea(a._a);this._isDSTShifted=this.isValid()&&s(a._a,b.toArray())>0}else this._isDSTShifted=!1;return this._isDSTShifted}function Wa(){return this.isValid()?!this._isUTC:!1}function Xa(){return this.isValid()?this._isUTC:!1}function Ya(){return this.isValid()?this._isUTC&&0===this._offset:!1}function Za(a,b){var c,d,e,g=a,h=null;return Ja(a)?g={ms:a._milliseconds,d:a._days,M:a._months}:"number"==typeof a?(g={},b?g[b]=a:g.milliseconds=a):(h=Vd.exec(a))?(c="-"===h[1]?-1:1,g={y:0,d:r(h[xd])*c,h:r(h[yd])*c,m:r(h[zd])*c,s:r(h[Ad])*c,ms:r(h[Bd])*c}):(h=Wd.exec(a))?(c="-"===h[1]?-1:1,g={y:$a(h[2],c),M:$a(h[3],c),d:$a(h[4],c),h:$a(h[5],c),m:$a(h[6],c),s:$a(h[7],c),w:$a(h[8],c)}):null==g?g={}:"object"==typeof g&&("from"in g||"to"in g)&&(e=ab(Ea(g.from),Ea(g.to)),g={},g.ms=e.milliseconds,g.M=e.months),d=new Ia(g),Ja(a)&&f(a,"_locale")&&(d._locale=a._locale),d}function $a(a,b){var c=a&&parseFloat(a.replace(",","."));return(isNaN(c)?0:c)*b}function _a(a,b){var c={milliseconds:0,months:0};return c.months=b.month()-a.month()+12*(b.year()-a.year()),a.clone().add(c.months,"M").isAfter(b)&&--c.months,c.milliseconds=+b-+a.clone().add(c.months,"M"),c}function ab(a,b){var c;return a.isValid()&&b.isValid()?(b=Ma(b,a),a.isBefore(b)?c=_a(a,b):(c=_a(b,a),c.milliseconds=-c.milliseconds,c.months=-c.months),c):{milliseconds:0,months:0}}function bb(a,b){return function(c,d){var e,f;return null===d||isNaN(+d)||(ga(b,"moment()."+b+"(period, number) is deprecated. Please use moment()."+b+"(number, period)."),f=c,c=d,d=f),c="string"==typeof c?+c:c,e=Za(c,d),cb(this,e,a),this}}function cb(b,c,d,e){var f=c._milliseconds,g=c._days,h=c._months;b.isValid()&&(e=null==e?!0:e,f&&b._d.setTime(+b._d+f*d),g&&G(b,"Date",F(b,"Date")+g*d),h&&Z(b,F(b,"Month")+h*d),e&&a.updateOffset(b,g||h))}function db(a,b){var c=a||Ea(),d=Ma(c,this).startOf("day"),e=this.diff(d,"days",!0),f=-6>e?"sameElse":-1>e?"lastWeek":0>e?"lastDay":1>e?"sameDay":2>e?"nextDay":7>e?"nextWeek":"sameElse",g=b&&(D(b[f])?b[f]():b[f]);return this.format(g||this.localeData().calendar(f,this,Ea(c)))}function eb(){return new o(this)}function fb(a,b){var c=p(a)?a:Ea(a);return this.isValid()&&c.isValid()?(b=B(m(b)?"millisecond":b),"millisecond"===b?+this>+c:+c<+this.clone().startOf(b)):!1}function gb(a,b){var c=p(a)?a:Ea(a);return this.isValid()&&c.isValid()?(b=B(m(b)?"millisecond":b),"millisecond"===b?+c>+this:+this.clone().endOf(b)<+c):!1}function hb(a,b,c){return this.isAfter(a,c)&&this.isBefore(b,c)}function ib(a,b){var c,d=p(a)?a:Ea(a);return this.isValid()&&d.isValid()?(b=B(b||"millisecond"),"millisecond"===b?+this===+d:(c=+d,+this.clone().startOf(b)<=c&&c<=+this.clone().endOf(b))):!1}function jb(a,b){return this.isSame(a,b)||this.isAfter(a,b)}function kb(a,b){return this.isSame(a,b)||this.isBefore(a,b)}function lb(a,b,c){var d,e,f,g;return this.isValid()?(d=Ma(a,this),d.isValid()?(e=6e4*(d.utcOffset()-this.utcOffset()),b=B(b),"year"===b||"month"===b||"quarter"===b?(g=mb(this,d),"quarter"===b?g/=3:"year"===b&&(g/=12)):(f=this-d,g="second"===b?f/1e3:"minute"===b?f/6e4:"hour"===b?f/36e5:"day"===b?(f-e)/864e5:"week"===b?(f-e)/6048e5:f),c?g:q(g)):NaN):NaN}function mb(a,b){var c,d,e=12*(b.year()-a.year())+(b.month()-a.month()),f=a.clone().add(e,"months");return 0>b-f?(c=a.clone().add(e-1,"months"),d=(b-f)/(f-c)):(c=a.clone().add(e+1,"months"),d=(b-f)/(c-f)),-(e+d)}function nb(){return this.clone().locale("en").format("ddd MMM DD YYYY HH:mm:ss [GMT]ZZ")}function ob(){var a=this.clone().utc();return 0<a.year()&&a.year()<=9999?D(Date.prototype.toISOString)?this.toDate().toISOString():M(a,"YYYY-MM-DD[T]HH:mm:ss.SSS[Z]"):M(a,"YYYYYY-MM-DD[T]HH:mm:ss.SSS[Z]")}function pb(b){var c=M(this,b||a.defaultFormat);return this.localeData().postformat(c)}function qb(a,b){return this.isValid()&&(p(a)&&a.isValid()||Ea(a).isValid())?Za({to:this,from:a}).locale(this.locale()).humanize(!b):this.localeData().invalidDate()}function rb(a){return this.from(Ea(),a)}function sb(a,b){return this.isValid()&&(p(a)&&a.isValid()||Ea(a).isValid())?Za({from:this,to:a}).locale(this.locale()).humanize(!b):this.localeData().invalidDate()}function tb(a){return this.to(Ea(),a)}function ub(a){var b;return void 0===a?this._locale._abbr:(b=z(a),null!=b&&(this._locale=b),this)}function vb(){return this._locale}function wb(a){switch(a=B(a)){case"year":this.month(0);case"quarter":case"month":this.date(1);case"week":case"isoWeek":case"day":this.hours(0);case"hour":this.minutes(0);case"minute":this.seconds(0);case"second":this.milliseconds(0)}return"week"===a&&this.weekday(0),"isoWeek"===a&&this.isoWeekday(1),"quarter"===a&&this.month(3*Math.floor(this.month()/3)),this}function xb(a){return a=B(a),void 0===a||"millisecond"===a?this:this.startOf(a).add(1,"isoWeek"===a?"week":a).subtract(1,"ms")}function yb(){return+this._d-6e4*(this._offset||0)}function zb(){return Math.floor(+this/1e3)}function Ab(){return this._offset?new Date(+this):this._d}function Bb(){var a=this;return[a.year(),a.month(),a.date(),a.hour(),a.minute(),a.second(),a.millisecond()]}function Cb(){var a=this;return{years:a.year(),months:a.month(),date:a.date(),hours:a.hours(),minutes:a.minutes(),seconds:a.seconds(),milliseconds:a.milliseconds()}}function Db(){return this.isValid()?this.toISOString():"null"}function Eb(){return k(this)}function Fb(){return g({},j(this))}function Gb(){return j(this).overflow}function Hb(){return{input:this._i,format:this._f,locale:this._locale,isUTC:this._isUTC,strict:this._strict}}function Ib(a,b){J(0,[a,a.length],0,b)}function Jb(a){return Nb.call(this,a,this.week(),this.weekday(),this.localeData()._week.dow,this.localeData()._week.doy)}function Kb(a){return Nb.call(this,a,this.isoWeek(),this.isoWeekday(),1,4)}function Lb(){return ra(this.year(),1,4)}function Mb(){var a=this.localeData()._week;return ra(this.year(),a.dow,a.doy)}function Nb(a,b,c,d,e){var f;return null==a?qa(this,d,e).year:(f=ra(a,d,e),b>f&&(b=f),Ob.call(this,a,b,c,d,e))}function Ob(a,b,c,d,e){var f=pa(a,b,c,d,e),g=ka(f.year,0,f.dayOfYear);return this.year(g.getUTCFullYear()),this.month(g.getUTCMonth()),this.date(g.getUTCDate()),this}function Pb(a){return null==a?Math.ceil((this.month()+1)/3):this.month(3*(a-1)+this.month()%3)}function Qb(a){return qa(a,this._week.dow,this._week.doy).week}function Rb(){return this._week.dow}function Sb(){return this._week.doy}function Tb(a){var b=this.localeData().week(this);return null==a?b:this.add(7*(a-b),"d")}function Ub(a){var b=qa(this,1,4).week;return null==a?b:this.add(7*(a-b),"d")}function Vb(a,b){return"string"!=typeof a?a:isNaN(a)?(a=b.weekdaysParse(a),"number"==typeof a?a:null):parseInt(a,10)}function Wb(a,b){return c(this._weekdays)?this._weekdays[a.day()]:this._weekdays[this._weekdays.isFormat.test(b)?"format":"standalone"][a.day()]}function Xb(a){return this._weekdaysShort[a.day()]}function Yb(a){return this._weekdaysMin[a.day()]}function Zb(a,b,c){var d,e,f;for(this._weekdaysParse||(this._weekdaysParse=[],this._minWeekdaysParse=[],this._shortWeekdaysParse=[],this._fullWeekdaysParse=[]),d=0;7>d;d++){if(e=Ea([2e3,1]).day(d),c&&!this._fullWeekdaysParse[d]&&(this._fullWeekdaysParse[d]=new RegExp("^"+this.weekdays(e,"").replace(".",".?")+"$","i"),this._shortWeekdaysParse[d]=new RegExp("^"+this.weekdaysShort(e,"").replace(".",".?")+"$","i"),this._minWeekdaysParse[d]=new RegExp("^"+this.weekdaysMin(e,"").replace(".",".?")+"$","i")),this._weekdaysParse[d]||(f="^"+this.weekdays(e,"")+"|^"+this.weekdaysShort(e,"")+"|^"+this.weekdaysMin(e,""),this._weekdaysParse[d]=new RegExp(f.replace(".",""),"i")),c&&"dddd"===b&&this._fullWeekdaysParse[d].test(a))return d;if(c&&"ddd"===b&&this._shortWeekdaysParse[d].test(a))return d;if(c&&"dd"===b&&this._minWeekdaysParse[d].test(a))return d;if(!c&&this._weekdaysParse[d].test(a))return d}}function $b(a){if(!this.isValid())return null!=a?this:NaN;var b=this._isUTC?this._d.getUTCDay():this._d.getDay();return null!=a?(a=Vb(a,this.localeData()),this.add(a-b,"d")):b}function _b(a){if(!this.isValid())return null!=a?this:NaN;var b=(this.day()+7-this.localeData()._week.dow)%7;return null==a?b:this.add(a-b,"d")}function ac(a){return this.isValid()?null==a?this.day()||7:this.day(this.day()%7?a:a-7):null!=a?this:NaN}function bc(a){var b=Math.round((this.clone().startOf("day")-this.clone().startOf("year"))/864e5)+1;return null==a?b:this.add(a-b,"d")}function cc(){return this.hours()%12||12}function dc(a,b){J(a,0,0,function(){return this.localeData().meridiem(this.hours(),this.minutes(),b)})}function ec(a,b){return b._meridiemParse}function fc(a){return"p"===(a+"").toLowerCase().charAt(0)}function gc(a,b,c){return a>11?c?"pm":"PM":c?"am":"AM"}function hc(a,b){b[Bd]=r(1e3*("0."+a))}function ic(){return this._isUTC?"UTC":""}function jc(){return this._isUTC?"Coordinated Universal Time":""}function kc(a){return Ea(1e3*a)}function lc(){return Ea.apply(null,arguments).parseZone()}function mc(a,b,c){var d=this._calendar[a];return D(d)?d.call(b,c):d}function nc(a){var b=this._longDateFormat[a],c=this._longDateFormat[a.toUpperCase()];return b||!c?b:(this._longDateFormat[a]=c.replace(/MMMM|MM|DD|dddd/g,function(a){return a.slice(1)}),this._longDateFormat[a])}function oc(){return this._invalidDate}function pc(a){return this._ordinal.replace("%d",a)}function qc(a){return a}function rc(a,b,c,d){var e=this._relativeTime[c];return D(e)?e(a,b,c,d):e.replace(/%d/i,a)}function sc(a,b){var c=this._relativeTime[a>0?"future":"past"];return D(c)?c(b):c.replace(/%s/i,b)}function tc(a){var b,c;for(c in a)b=a[c],D(b)?this[c]=b:this["_"+c]=b;this._ordinalParseLenient=new RegExp(this._ordinalParse.source+"|"+/\d{1,2}/.source)}function uc(a,b,c,d){var e=z(),f=h().set(d,b);return e[c](f,a)}function vc(a,b,c,d,e){if("number"==typeof a&&(b=a,a=void 0),a=a||"",null!=b)return uc(a,b,c,e);var f,g=[];for(f=0;d>f;f++)g[f]=uc(a,f,c,e);return g}function wc(a,b){return vc(a,b,"months",12,"month")}function xc(a,b){return vc(a,b,"monthsShort",12,"month")}function yc(a,b){return vc(a,b,"weekdays",7,"day")}function zc(a,b){return vc(a,b,"weekdaysShort",7,"day")}function Ac(a,b){return vc(a,b,"weekdaysMin",7,"day")}function Bc(){var a=this._data;return this._milliseconds=se(this._milliseconds),this._days=se(this._days),this._months=se(this._months),a.milliseconds=se(a.milliseconds),a.seconds=se(a.seconds),a.minutes=se(a.minutes),a.hours=se(a.hours),a.months=se(a.months),a.years=se(a.years),this}function Cc(a,b,c,d){var e=Za(b,c);return a._milliseconds+=d*e._milliseconds,a._days+=d*e._days,a._months+=d*e._months,a._bubble()}function Dc(a,b){return Cc(this,a,b,1)}function Ec(a,b){return Cc(this,a,b,-1)}function Fc(a){return 0>a?Math.floor(a):Math.ceil(a)}function Gc(){var a,b,c,d,e,f=this._milliseconds,g=this._days,h=this._months,i=this._data;return f>=0&&g>=0&&h>=0||0>=f&&0>=g&&0>=h||(f+=864e5*Fc(Ic(h)+g),g=0,h=0),i.milliseconds=f%1e3,a=q(f/1e3),i.seconds=a%60,b=q(a/60),i.minutes=b%60,c=q(b/60),i.hours=c%24,g+=q(c/24),e=q(Hc(g)),h+=e,g-=Fc(Ic(e)),d=q(h/12),h%=12,i.days=g,i.months=h,i.years=d,this}function Hc(a){return 4800*a/146097}function Ic(a){return 146097*a/4800}function Jc(a){var b,c,d=this._milliseconds;if(a=B(a),"month"===a||"year"===a)return b=this._days+d/864e5,c=this._months+Hc(b),"month"===a?c:c/12;switch(b=this._days+Math.round(Ic(this._months)),a){case"week":return b/7+d/6048e5;case"day":return b+d/864e5;case"hour":return 24*b+d/36e5;case"minute":return 1440*b+d/6e4;case"second":return 86400*b+d/1e3;case"millisecond":return Math.floor(864e5*b)+d;default:throw new Error("Unknown unit "+a)}}function Kc(){return this._milliseconds+864e5*this._days+this._months%12*2592e6+31536e6*r(this._months/12)}function Lc(a){return function(){return this.as(a)}}function Mc(a){return a=B(a),this[a+"s"]()}function Nc(a){return function(){return this._data[a]}}function Oc(){return q(this.days()/7)}function Pc(a,b,c,d,e){return e.relativeTime(b||1,!!c,a,d)}function Qc(a,b,c){var d=Za(a).abs(),e=Ie(d.as("s")),f=Ie(d.as("m")),g=Ie(d.as("h")),h=Ie(d.as("d")),i=Ie(d.as("M")),j=Ie(d.as("y")),k=e<Je.s&&["s",e]||1>=f&&["m"]||f<Je.m&&["mm",f]||1>=g&&["h"]||g<Je.h&&["hh",g]||1>=h&&["d"]||h<Je.d&&["dd",h]||1>=i&&["M"]||i<Je.M&&["MM",i]||1>=j&&["y"]||["yy",j];return k[2]=b,k[3]=+a>0,k[4]=c,Pc.apply(null,k)}function Rc(a,b){return void 0===Je[a]?!1:void 0===b?Je[a]:(Je[a]=b,!0)}function Sc(a){var b=this.localeData(),c=Qc(this,!a,b);return a&&(c=b.pastFuture(+this,c)),b.postformat(c)}function Tc(){var a,b,c,d=Ke(this._milliseconds)/1e3,e=Ke(this._days),f=Ke(this._months);a=q(d/60),b=q(a/60),d%=60,a%=60,c=q(f/12),f%=12;var g=c,h=f,i=e,j=b,k=a,l=d,m=this.asSeconds();return m?(0>m?"-":"")+"P"+(g?g+"Y":"")+(h?h+"M":"")+(i?i+"D":"")+(j||k||l?"T":"")+(j?j+"H":"")+(k?k+"M":"")+(l?l+"S":""):"P0D"}var Uc,Vc,Wc=a.momentProperties=[],Xc=!1,Yc={},Zc={},$c=/(\[[^\[]*\])|(\\)?([Hh]mm(ss)?|Mo|MM?M?M?|Do|DDDo|DD?D?D?|ddd?d?|do?|w[o|w]?|W[o|W]?|Qo?|YYYYYY|YYYYY|YYYY|YY|gg(ggg?)?|GG(GGG?)?|e|E|a|A|hh?|HH?|mm?|ss?|S{1,9}|x|X|zz?|ZZ?|.)/g,_c=/(\[[^\[]*\])|(\\)?(LTS|LT|LL?L?L?|l{1,4})/g,ad={},bd={},cd=/\d/,dd=/\d\d/,ed=/\d{3}/,fd=/\d{4}/,gd=/[+-]?\d{6}/,hd=/\d\d?/,id=/\d\d\d\d?/,jd=/\d\d\d\d\d\d?/,kd=/\d{1,3}/,ld=/\d{1,4}/,md=/[+-]?\d{1,6}/,nd=/\d+/,od=/[+-]?\d+/,pd=/Z|[+-]\d\d:?\d\d/gi,qd=/Z|[+-]\d\d(?::?\d\d)?/gi,rd=/[+-]?\d+(\.\d{1,3})?/,sd=/[0-9]*['a-z\u00A0-\u05FF\u0700-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+|[\u0600-\u06FF\/]+(\s*?[\u0600-\u06FF]+){1,2}/i,td={},ud={},vd=0,wd=1,xd=2,yd=3,zd=4,Ad=5,Bd=6,Cd=7,Dd=8;J("M",["MM",2],"Mo",function(){return this.month()+1}),J("MMM",0,0,function(a){return this.localeData().monthsShort(this,a)}),J("MMMM",0,0,function(a){return this.localeData().months(this,a)}),A("month","M"),O("M",hd),O("MM",hd,dd),O("MMM",function(a,b){return b.monthsShortRegex(a)}),O("MMMM",function(a,b){return b.monthsRegex(a)}),S(["M","MM"],function(a,b){b[wd]=r(a)-1}),S(["MMM","MMMM"],function(a,b,c,d){var e=c._locale.monthsParse(a,d,c._strict);null!=e?b[wd]=e:j(c).invalidMonth=a});var Ed=/D[oD]?(\[[^\[\]]*\]|\s+)+MMMM?/,Fd="January_February_March_April_May_June_July_August_September_October_November_December".split("_"),Gd="Jan_Feb_Mar_Apr_May_Jun_Jul_Aug_Sep_Oct_Nov_Dec".split("_"),Hd=sd,Id=sd,Jd={};a.suppressDeprecationWarnings=!1;var Kd=/^\s*((?:[+-]\d{6}|\d{4})-(?:\d\d-\d\d|W\d\d-\d|W\d\d|\d\d\d|\d\d))(?:(T| )(\d\d(?::\d\d(?::\d\d(?:[.,]\d+)?)?)?)([\+\-]\d\d(?::?\d\d)?|\s*Z)?)?/,Ld=/^\s*((?:[+-]\d{6}|\d{4})(?:\d\d\d\d|W\d\d\d|W\d\d|\d\d\d|\d\d))(?:(T| )(\d\d(?:\d\d(?:\d\d(?:[.,]\d+)?)?)?)([\+\-]\d\d(?::?\d\d)?|\s*Z)?)?/,Md=/Z|[+-]\d\d(?::?\d\d)?/,Nd=[["YYYYYY-MM-DD",/[+-]\d{6}-\d\d-\d\d/],["YYYY-MM-DD",/\d{4}-\d\d-\d\d/],["GGGG-[W]WW-E",/\d{4}-W\d\d-\d/],["GGGG-[W]WW",/\d{4}-W\d\d/,!1],["YYYY-DDD",/\d{4}-\d{3}/],["YYYY-MM",/\d{4}-\d\d/,!1],["YYYYYYMMDD",/[+-]\d{10}/],["YYYYMMDD",/\d{8}/],["GGGG[W]WWE",/\d{4}W\d{3}/],["GGGG[W]WW",/\d{4}W\d{2}/,!1],["YYYYDDD",/\d{7}/]],Od=[["HH:mm:ss.SSSS",/\d\d:\d\d:\d\d\.\d+/],["HH:mm:ss,SSSS",/\d\d:\d\d:\d\d,\d+/],["HH:mm:ss",/\d\d:\d\d:\d\d/],["HH:mm",/\d\d:\d\d/],["HHmmss.SSSS",/\d\d\d\d\d\d\.\d+/],["HHmmss,SSSS",/\d\d\d\d\d\d,\d+/],["HHmmss",/\d\d\d\d\d\d/],["HHmm",/\d\d\d\d/],["HH",/\d\d/]],Pd=/^\/?Date\((\-?\d+)/i;a.createFromInputFallback=fa("moment construction falls back to js Date. This is discouraged and will be removed in upcoming major release. Please refer to https://github.com/moment/moment/issues/1407 for more info.",function(a){a._d=new Date(a._i+(a._useUTC?" UTC":""))}),J("Y",0,0,function(){var a=this.year();return 9999>=a?""+a:"+"+a}),J(0,["YY",2],0,function(){return this.year()%100}),J(0,["YYYY",4],0,"year"),J(0,["YYYYY",5],0,"year"),J(0,["YYYYYY",6,!0],0,"year"),A("year","y"),O("Y",od),O("YY",hd,dd),O("YYYY",ld,fd),O("YYYYY",md,gd),O("YYYYYY",md,gd),S(["YYYYY","YYYYYY"],vd),S("YYYY",function(b,c){c[vd]=2===b.length?a.parseTwoDigitYear(b):r(b)}),S("YY",function(b,c){c[vd]=a.parseTwoDigitYear(b)}),S("Y",function(a,b){b[vd]=parseInt(a,10)}),a.parseTwoDigitYear=function(a){return r(a)+(r(a)>68?1900:2e3)};var Qd=E("FullYear",!1);a.ISO_8601=function(){};var Rd=fa("moment().min is deprecated, use moment.min instead. https://github.com/moment/moment/issues/1548",function(){var a=Ea.apply(null,arguments);return this.isValid()&&a.isValid()?this>a?this:a:l()}),Sd=fa("moment().max is deprecated, use moment.max instead. https://github.com/moment/moment/issues/1548",function(){var a=Ea.apply(null,arguments);return this.isValid()&&a.isValid()?a>this?this:a:l()}),Td=function(){return Date.now?Date.now():+new Date};Ka("Z",":"),Ka("ZZ",""),O("Z",qd),O("ZZ",qd),S(["Z","ZZ"],function(a,b,c){c._useUTC=!0,c._tzm=La(qd,a)});var Ud=/([\+\-]|\d\d)/gi;a.updateOffset=function(){};var Vd=/^(\-)?(?:(\d*)[. ])?(\d+)\:(\d+)(?:\:(\d+)\.?(\d{3})?\d*)?$/,Wd=/^(-)?P(?:(?:([0-9,.]*)Y)?(?:([0-9,.]*)M)?(?:([0-9,.]*)D)?(?:T(?:([0-9,.]*)H)?(?:([0-9,.]*)M)?(?:([0-9,.]*)S)?)?|([0-9,.]*)W)$/;
Za.fn=Ia.prototype;var Xd=bb(1,"add"),Yd=bb(-1,"subtract");a.defaultFormat="YYYY-MM-DDTHH:mm:ssZ";var Zd=fa("moment().lang() is deprecated. Instead, use moment().localeData() to get the language configuration. Use moment().locale() to change languages.",function(a){return void 0===a?this.localeData():this.locale(a)});J(0,["gg",2],0,function(){return this.weekYear()%100}),J(0,["GG",2],0,function(){return this.isoWeekYear()%100}),Ib("gggg","weekYear"),Ib("ggggg","weekYear"),Ib("GGGG","isoWeekYear"),Ib("GGGGG","isoWeekYear"),A("weekYear","gg"),A("isoWeekYear","GG"),O("G",od),O("g",od),O("GG",hd,dd),O("gg",hd,dd),O("GGGG",ld,fd),O("gggg",ld,fd),O("GGGGG",md,gd),O("ggggg",md,gd),T(["gggg","ggggg","GGGG","GGGGG"],function(a,b,c,d){b[d.substr(0,2)]=r(a)}),T(["gg","GG"],function(b,c,d,e){c[e]=a.parseTwoDigitYear(b)}),J("Q",0,"Qo","quarter"),A("quarter","Q"),O("Q",cd),S("Q",function(a,b){b[wd]=3*(r(a)-1)}),J("w",["ww",2],"wo","week"),J("W",["WW",2],"Wo","isoWeek"),A("week","w"),A("isoWeek","W"),O("w",hd),O("ww",hd,dd),O("W",hd),O("WW",hd,dd),T(["w","ww","W","WW"],function(a,b,c,d){b[d.substr(0,1)]=r(a)});var $d={dow:0,doy:6};J("D",["DD",2],"Do","date"),A("date","D"),O("D",hd),O("DD",hd,dd),O("Do",function(a,b){return a?b._ordinalParse:b._ordinalParseLenient}),S(["D","DD"],xd),S("Do",function(a,b){b[xd]=r(a.match(hd)[0],10)});var _d=E("Date",!0);J("d",0,"do","day"),J("dd",0,0,function(a){return this.localeData().weekdaysMin(this,a)}),J("ddd",0,0,function(a){return this.localeData().weekdaysShort(this,a)}),J("dddd",0,0,function(a){return this.localeData().weekdays(this,a)}),J("e",0,0,"weekday"),J("E",0,0,"isoWeekday"),A("day","d"),A("weekday","e"),A("isoWeekday","E"),O("d",hd),O("e",hd),O("E",hd),O("dd",sd),O("ddd",sd),O("dddd",sd),T(["dd","ddd","dddd"],function(a,b,c,d){var e=c._locale.weekdaysParse(a,d,c._strict);null!=e?b.d=e:j(c).invalidWeekday=a}),T(["d","e","E"],function(a,b,c,d){b[d]=r(a)});var ae="Sunday_Monday_Tuesday_Wednesday_Thursday_Friday_Saturday".split("_"),be="Sun_Mon_Tue_Wed_Thu_Fri_Sat".split("_"),ce="Su_Mo_Tu_We_Th_Fr_Sa".split("_");J("DDD",["DDDD",3],"DDDo","dayOfYear"),A("dayOfYear","DDD"),O("DDD",kd),O("DDDD",ed),S(["DDD","DDDD"],function(a,b,c){c._dayOfYear=r(a)}),J("H",["HH",2],0,"hour"),J("h",["hh",2],0,cc),J("hmm",0,0,function(){return""+cc.apply(this)+I(this.minutes(),2)}),J("hmmss",0,0,function(){return""+cc.apply(this)+I(this.minutes(),2)+I(this.seconds(),2)}),J("Hmm",0,0,function(){return""+this.hours()+I(this.minutes(),2)}),J("Hmmss",0,0,function(){return""+this.hours()+I(this.minutes(),2)+I(this.seconds(),2)}),dc("a",!0),dc("A",!1),A("hour","h"),O("a",ec),O("A",ec),O("H",hd),O("h",hd),O("HH",hd,dd),O("hh",hd,dd),O("hmm",id),O("hmmss",jd),O("Hmm",id),O("Hmmss",jd),S(["H","HH"],yd),S(["a","A"],function(a,b,c){c._isPm=c._locale.isPM(a),c._meridiem=a}),S(["h","hh"],function(a,b,c){b[yd]=r(a),j(c).bigHour=!0}),S("hmm",function(a,b,c){var d=a.length-2;b[yd]=r(a.substr(0,d)),b[zd]=r(a.substr(d)),j(c).bigHour=!0}),S("hmmss",function(a,b,c){var d=a.length-4,e=a.length-2;b[yd]=r(a.substr(0,d)),b[zd]=r(a.substr(d,2)),b[Ad]=r(a.substr(e)),j(c).bigHour=!0}),S("Hmm",function(a,b,c){var d=a.length-2;b[yd]=r(a.substr(0,d)),b[zd]=r(a.substr(d))}),S("Hmmss",function(a,b,c){var d=a.length-4,e=a.length-2;b[yd]=r(a.substr(0,d)),b[zd]=r(a.substr(d,2)),b[Ad]=r(a.substr(e))});var de=/[ap]\.?m?\.?/i,ee=E("Hours",!0);J("m",["mm",2],0,"minute"),A("minute","m"),O("m",hd),O("mm",hd,dd),S(["m","mm"],zd);var fe=E("Minutes",!1);J("s",["ss",2],0,"second"),A("second","s"),O("s",hd),O("ss",hd,dd),S(["s","ss"],Ad);var ge=E("Seconds",!1);J("S",0,0,function(){return~~(this.millisecond()/100)}),J(0,["SS",2],0,function(){return~~(this.millisecond()/10)}),J(0,["SSS",3],0,"millisecond"),J(0,["SSSS",4],0,function(){return 10*this.millisecond()}),J(0,["SSSSS",5],0,function(){return 100*this.millisecond()}),J(0,["SSSSSS",6],0,function(){return 1e3*this.millisecond()}),J(0,["SSSSSSS",7],0,function(){return 1e4*this.millisecond()}),J(0,["SSSSSSSS",8],0,function(){return 1e5*this.millisecond()}),J(0,["SSSSSSSSS",9],0,function(){return 1e6*this.millisecond()}),A("millisecond","ms"),O("S",kd,cd),O("SS",kd,dd),O("SSS",kd,ed);var he;for(he="SSSS";he.length<=9;he+="S")O(he,nd);for(he="S";he.length<=9;he+="S")S(he,hc);var ie=E("Milliseconds",!1);J("z",0,0,"zoneAbbr"),J("zz",0,0,"zoneName");var je=o.prototype;je.add=Xd,je.calendar=db,je.clone=eb,je.diff=lb,je.endOf=xb,je.format=pb,je.from=qb,je.fromNow=rb,je.to=sb,je.toNow=tb,je.get=H,je.invalidAt=Gb,je.isAfter=fb,je.isBefore=gb,je.isBetween=hb,je.isSame=ib,je.isSameOrAfter=jb,je.isSameOrBefore=kb,je.isValid=Eb,je.lang=Zd,je.locale=ub,je.localeData=vb,je.max=Sd,je.min=Rd,je.parsingFlags=Fb,je.set=H,je.startOf=wb,je.subtract=Yd,je.toArray=Bb,je.toObject=Cb,je.toDate=Ab,je.toISOString=ob,je.toJSON=Db,je.toString=nb,je.unix=zb,je.valueOf=yb,je.creationData=Hb,je.year=Qd,je.isLeapYear=na,je.weekYear=Jb,je.isoWeekYear=Kb,je.quarter=je.quarters=Pb,je.month=$,je.daysInMonth=_,je.week=je.weeks=Tb,je.isoWeek=je.isoWeeks=Ub,je.weeksInYear=Mb,je.isoWeeksInYear=Lb,je.date=_d,je.day=je.days=$b,je.weekday=_b,je.isoWeekday=ac,je.dayOfYear=bc,je.hour=je.hours=ee,je.minute=je.minutes=fe,je.second=je.seconds=ge,je.millisecond=je.milliseconds=ie,je.utcOffset=Oa,je.utc=Qa,je.local=Ra,je.parseZone=Sa,je.hasAlignedHourOffset=Ta,je.isDST=Ua,je.isDSTShifted=Va,je.isLocal=Wa,je.isUtcOffset=Xa,je.isUtc=Ya,je.isUTC=Ya,je.zoneAbbr=ic,je.zoneName=jc,je.dates=fa("dates accessor is deprecated. Use date instead.",_d),je.months=fa("months accessor is deprecated. Use month instead",$),je.years=fa("years accessor is deprecated. Use year instead",Qd),je.zone=fa("moment().zone is deprecated, use moment().utcOffset instead. https://github.com/moment/moment/issues/1779",Pa);var ke=je,le={sameDay:"[Today at] LT",nextDay:"[Tomorrow at] LT",nextWeek:"dddd [at] LT",lastDay:"[Yesterday at] LT",lastWeek:"[Last] dddd [at] LT",sameElse:"L"},me={LTS:"h:mm:ss A",LT:"h:mm A",L:"MM/DD/YYYY",LL:"MMMM D, YYYY",LLL:"MMMM D, YYYY h:mm A",LLLL:"dddd, MMMM D, YYYY h:mm A"},ne="Invalid date",oe="%d",pe=/\d{1,2}/,qe={future:"in %s",past:"%s ago",s:"a few seconds",m:"a minute",mm:"%d minutes",h:"an hour",hh:"%d hours",d:"a day",dd:"%d days",M:"a month",MM:"%d months",y:"a year",yy:"%d years"},re=t.prototype;re._calendar=le,re.calendar=mc,re._longDateFormat=me,re.longDateFormat=nc,re._invalidDate=ne,re.invalidDate=oc,re._ordinal=oe,re.ordinal=pc,re._ordinalParse=pe,re.preparse=qc,re.postformat=qc,re._relativeTime=qe,re.relativeTime=rc,re.pastFuture=sc,re.set=tc,re.months=W,re._months=Fd,re.monthsShort=X,re._monthsShort=Gd,re.monthsParse=Y,re._monthsRegex=Id,re.monthsRegex=ba,re._monthsShortRegex=Hd,re.monthsShortRegex=aa,re.week=Qb,re._week=$d,re.firstDayOfYear=Sb,re.firstDayOfWeek=Rb,re.weekdays=Wb,re._weekdays=ae,re.weekdaysMin=Yb,re._weekdaysMin=ce,re.weekdaysShort=Xb,re._weekdaysShort=be,re.weekdaysParse=Zb,re.isPM=fc,re._meridiemParse=de,re.meridiem=gc,x("en",{ordinalParse:/\d{1,2}(th|st|nd|rd)/,ordinal:function(a){var b=a%10,c=1===r(a%100/10)?"th":1===b?"st":2===b?"nd":3===b?"rd":"th";return a+c}}),a.lang=fa("moment.lang is deprecated. Use moment.locale instead.",x),a.langData=fa("moment.langData is deprecated. Use moment.localeData instead.",z);var se=Math.abs,te=Lc("ms"),ue=Lc("s"),ve=Lc("m"),we=Lc("h"),xe=Lc("d"),ye=Lc("w"),ze=Lc("M"),Ae=Lc("y"),Be=Nc("milliseconds"),Ce=Nc("seconds"),De=Nc("minutes"),Ee=Nc("hours"),Fe=Nc("days"),Ge=Nc("months"),He=Nc("years"),Ie=Math.round,Je={s:45,m:45,h:22,d:26,M:11},Ke=Math.abs,Le=Ia.prototype;Le.abs=Bc,Le.add=Dc,Le.subtract=Ec,Le.as=Jc,Le.asMilliseconds=te,Le.asSeconds=ue,Le.asMinutes=ve,Le.asHours=we,Le.asDays=xe,Le.asWeeks=ye,Le.asMonths=ze,Le.asYears=Ae,Le.valueOf=Kc,Le._bubble=Gc,Le.get=Mc,Le.milliseconds=Be,Le.seconds=Ce,Le.minutes=De,Le.hours=Ee,Le.days=Fe,Le.weeks=Oc,Le.months=Ge,Le.years=He,Le.humanize=Sc,Le.toISOString=Tc,Le.toString=Tc,Le.toJSON=Tc,Le.locale=ub,Le.localeData=vb,Le.toIsoString=fa("toIsoString() is deprecated. Please use toISOString() instead (notice the capitals)",Tc),Le.lang=Zd,J("X",0,0,"unix"),J("x",0,0,"valueOf"),O("x",od),O("X",rd),S("X",function(a,b,c){c._d=new Date(1e3*parseFloat(a,10))}),S("x",function(a,b,c){c._d=new Date(r(a))}),a.version="2.11.2",b(Ea),a.fn=ke,a.min=Ga,a.max=Ha,a.now=Td,a.utc=h,a.unix=kc,a.months=wc,a.isDate=d,a.locale=x,a.invalid=l,a.duration=Za,a.isMoment=p,a.weekdays=yc,a.parseZone=lc,a.localeData=z,a.isDuration=Ja,a.monthsShort=xc,a.weekdaysMin=Ac,a.defineLocale=y,a.weekdaysShort=zc,a.normalizeUnits=B,a.relativeTimeThreshold=Rc,a.prototype=ke;var Me=a;return Me});
/*!
 * Pikaday
 *
 * Copyright © 2014 David Bushell | BSD & MIT license | https://github.com/dbushell/Pikaday
 */

(function (root, factory)
{
    'use strict';

    var moment;
    if (typeof exports === 'object') {
        // CommonJS module
        // Load moment.js as an optional dependency
        try { moment = require('moment'); } catch (e) {}
        module.exports = factory(moment);
    } else if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(function (req)
        {
            // Load moment.js as an optional dependency
            var id = 'moment';
            try { moment = req(id); } catch (e) {}
            return factory(moment);
        });
    } else {
        root.Pikaday = factory(root.moment);
    }
}(this, function (moment)
{
    'use strict';

    /**
     * feature detection and helper functions
     */
    var hasMoment = typeof moment === 'function',

    hasEventListeners = !!window.addEventListener,

    document = window.document,

    sto = window.setTimeout,

    addEvent = function(el, e, callback, capture)
    {
        if (hasEventListeners) {
            el.addEventListener(e, callback, !!capture);
        } else {
            el.attachEvent('on' + e, callback);
        }
    },

    removeEvent = function(el, e, callback, capture)
    {
        if (hasEventListeners) {
            el.removeEventListener(e, callback, !!capture);
        } else {
            el.detachEvent('on' + e, callback);
        }
    },

    fireEvent = function(el, eventName, data)
    {
        var ev;

        if (document.createEvent) {
            ev = document.createEvent('HTMLEvents');
            ev.initEvent(eventName, true, false);
            ev = extend(ev, data);
            el.dispatchEvent(ev);
        } else if (document.createEventObject) {
            ev = document.createEventObject();
            ev = extend(ev, data);
            el.fireEvent('on' + eventName, ev);
        }
    },

    trim = function(str)
    {
        return str.trim ? str.trim() : str.replace(/^\s+|\s+$/g,'');
    },

    hasClass = function(el, cn)
    {
        return (' ' + el.className + ' ').indexOf(' ' + cn + ' ') !== -1;
    },

    addClass = function(el, cn)
    {
        if (!hasClass(el, cn)) {
            el.className = (el.className === '') ? cn : el.className + ' ' + cn;
        }
    },

    removeClass = function(el, cn)
    {
        el.className = trim((' ' + el.className + ' ').replace(' ' + cn + ' ', ' '));
    },

    isArray = function(obj)
    {
        return (/Array/).test(Object.prototype.toString.call(obj));
    },

    isDate = function(obj)
    {
        return (/Date/).test(Object.prototype.toString.call(obj)) && !isNaN(obj.getTime());
    },

    isWeekend = function(date)
    {
        var day = date.getDay();
        return day === 0 || day === 6;
    },

    isLeapYear = function(year)
    {
        // solution by Matti Virkkunen: http://stackoverflow.com/a/4881951
        return year % 4 === 0 && year % 100 !== 0 || year % 400 === 0;
    },

    getDaysInMonth = function(year, month)
    {
        return [31, isLeapYear(year) ? 29 : 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31][month];
    },

    setToStartOfDay = function(date)
    {
        if (isDate(date)) date.setHours(0,0,0,0);
    },

    compareDates = function(a,b)
    {
        // weak date comparison (use setToStartOfDay(date) to ensure correct result)
        return a.getTime() === b.getTime();
    },

    extend = function(to, from, overwrite)
    {
        var prop, hasProp;
        for (prop in from) {
            hasProp = to[prop] !== undefined;
            if (hasProp && typeof from[prop] === 'object' && from[prop] !== null && from[prop].nodeName === undefined) {
                if (isDate(from[prop])) {
                    if (overwrite) {
                        to[prop] = new Date(from[prop].getTime());
                    }
                }
                else if (isArray(from[prop])) {
                    if (overwrite) {
                        to[prop] = from[prop].slice(0);
                    }
                } else {
                    to[prop] = extend({}, from[prop], overwrite);
                }
            } else if (overwrite || !hasProp) {
                to[prop] = from[prop];
            }
        }
        return to;
    },

    adjustCalendar = function(calendar) {
        if (calendar.month < 0) {
            calendar.year -= Math.ceil(Math.abs(calendar.month)/12);
            calendar.month += 12;
        }
        if (calendar.month > 11) {
            calendar.year += Math.floor(Math.abs(calendar.month)/12);
            calendar.month -= 12;
        }
        return calendar;
    },

    /**
     * defaults and localisation
     */
    defaults = {

        // bind the picker to a form field
        field: null,

        // automatically show/hide the picker on `field` focus (default `true` if `field` is set)
        bound: undefined,

        // position of the datepicker, relative to the field (default to bottom & left)
        // ('bottom' & 'left' keywords are not used, 'top' & 'right' are modifier on the bottom/left position)
        position: 'bottom left',

        // automatically fit in the viewport even if it means repositioning from the position option
        reposition: true,

        // the default output format for `.toString()` and `field` value
        format: 'YYYY-MM-DD',

        // the initial date to view when first opened
        defaultDate: null,

        // make the `defaultDate` the initial selected value
        setDefaultDate: false,

        // first day of week (0: Sunday, 1: Monday etc)
        firstDay: 0,

        // the minimum/earliest date that can be selected
        minDate: null,
        // the maximum/latest date that can be selected
        maxDate: null,

        // number of years either side, or array of upper/lower range
        yearRange: 10,

        // show week numbers at head of row
        showWeekNumber: false,

        // used internally (don't config outside)
        minYear: 0,
        maxYear: 9999,
        minMonth: undefined,
        maxMonth: undefined,

        startRange: null,
        endRange: null,

        isRTL: false,

        // Additional text to append to the year in the calendar title
        yearSuffix: '',

        // Render the month after year in the calendar title
        showMonthAfterYear: false,

        // how many months are visible
        numberOfMonths: 1,

        // when numberOfMonths is used, this will help you to choose where the main calendar will be (default `left`, can be set to `right`)
        // only used for the first display or when a selected date is not visible
        mainCalendar: 'left',

        // Specify a DOM element to render the calendar in
        container: undefined,

        // internationalization
        i18n: {
            previousMonth : 'Previous Month',
            nextMonth     : 'Next Month',
            months        : ['January','February','March','April','May','June','July','August','September','October','November','December'],
            weekdays      : ['Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday'],
            weekdaysShort : ['Sun','Mon','Tue','Wed','Thu','Fri','Sat']
        },

        // Theme Classname
        theme: null,

        // callback function
        onSelect: null,
        onOpen: null,
        onClose: null,
        onDraw: null
    },


    /**
     * templating functions to abstract HTML rendering
     */
    renderDayName = function(opts, day, abbr)
    {
        day += opts.firstDay;
        while (day >= 7) {
            day -= 7;
        }
        return abbr ? opts.i18n.weekdaysShort[day] : opts.i18n.weekdays[day];
    },

    renderDay = function(opts)
    {
        if (opts.isEmpty) {
            return '<td class="is-empty"></td>';
        }
        var arr = [];
        if (opts.isDisabled) {
            arr.push('is-disabled');
        }
        if (opts.isToday) {
            arr.push('is-today');
        }
        if (opts.isSelected) {
            arr.push('is-selected');
        }
        if (opts.isInRange) {
            arr.push('is-inrange');
        }
        if (opts.isStartRange) {
            arr.push('is-startrange');
        }
        if (opts.isEndRange) {
            arr.push('is-endrange');
        }
        return '<td data-day="' + opts.day + '" class="' + arr.join(' ') + '">' +
                 '<button class="pika-button pika-day" type="button" ' +
                    'data-pika-year="' + opts.year + '" data-pika-month="' + opts.month + '" data-pika-day="' + opts.day + '">' +
                        opts.day +
                 '</button>' +
               '</td>';
    },

    renderWeek = function (d, m, y) {
        // Lifted from http://javascript.about.com/library/blweekyear.htm, lightly modified.
        var onejan = new Date(y, 0, 1),
            weekNum = Math.ceil((((new Date(y, m, d) - onejan) / 86400000) + onejan.getDay()+1)/7);
        return '<td class="pika-week">' + weekNum + '</td>';
    },

    renderRow = function(days, isRTL)
    {
        return '<tr>' + (isRTL ? days.reverse() : days).join('') + '</tr>';
    },

    renderBody = function(rows)
    {
        return '<tbody>' + rows.join('') + '</tbody>';
    },

    renderHead = function(opts)
    {
        var i, arr = [];
        if (opts.showWeekNumber) {
            arr.push('<th></th>');
        }
        for (i = 0; i < 7; i++) {
            arr.push('<th scope="col"><abbr title="' + renderDayName(opts, i) + '">' + renderDayName(opts, i, true) + '</abbr></th>');
        }
        return '<thead>' + (opts.isRTL ? arr.reverse() : arr).join('') + '</thead>';
    },

    renderTitle = function(instance, c, year, month, refYear)
    {
        var i, j, arr,
            opts = instance._o,
            isMinYear = year === opts.minYear,
            isMaxYear = year === opts.maxYear,
            html = '<div class="pika-title">',
            monthHtml,
            yearHtml,
            prev = true,
            next = true;

        for (arr = [], i = 0; i < 12; i++) {
            arr.push('<option value="' + (year === refYear ? i - c : 12 + i - c) + '"' +
                (i === month ? ' selected': '') +
                ((isMinYear && i < opts.minMonth) || (isMaxYear && i > opts.maxMonth) ? 'disabled' : '') + '>' +
                opts.i18n.months[i] + '</option>');
        }
        monthHtml = '<div class="pika-label">' + opts.i18n.months[month] + '<select class="pika-select pika-select-month" tabindex="-1">' + arr.join('') + '</select></div>';

        if (isArray(opts.yearRange)) {
            i = opts.yearRange[0];
            j = opts.yearRange[1] + 1;
        } else {
            i = year - opts.yearRange;
            j = 1 + year + opts.yearRange;
        }

        for (arr = []; i < j && i <= opts.maxYear; i++) {
            if (i >= opts.minYear) {
                arr.push('<option value="' + i + '"' + (i === year ? ' selected': '') + '>' + (i) + '</option>');
            }
        }
        yearHtml = '<div class="pika-label">' + year + opts.yearSuffix + '<select class="pika-select pika-select-year" tabindex="-1">' + arr.join('') + '</select></div>';

        if (opts.showMonthAfterYear) {
            html += yearHtml + monthHtml;
        } else {
            html += monthHtml + yearHtml;
        }

        if (isMinYear && (month === 0 || opts.minMonth >= month)) {
            prev = false;
        }

        if (isMaxYear && (month === 11 || opts.maxMonth <= month)) {
            next = false;
        }

        if (c === 0) {
            html += '<button class="pika-prev' + (prev ? '' : ' is-disabled') + '" type="button">' + opts.i18n.previousMonth + '</button>';
        }
        if (c === (instance._o.numberOfMonths - 1) ) {
            html += '<button class="pika-next' + (next ? '' : ' is-disabled') + '" type="button">' + opts.i18n.nextMonth + '</button>';
        }

        return html += '</div>';
    },

    renderTable = function(opts, data)
    {
        return '<table cellpadding="0" cellspacing="0" class="pika-table">' + renderHead(opts) + renderBody(data) + '</table>';
    },


    /**
     * Pikaday constructor
     */
    Pikaday = function(options)
    {
        var self = this,
            opts = self.config(options);

        self._onMouseDown = function(e)
        {
            if (!self._v) {
                return;
            }
            e = e || window.event;
            var target = e.target || e.srcElement;
            if (!target) {
                return;
            }

            if (!hasClass(target, 'is-disabled')) {
                if (hasClass(target, 'pika-button') && !hasClass(target, 'is-empty')) {
                    self.setDate(new Date(target.getAttribute('data-pika-year'), target.getAttribute('data-pika-month'), target.getAttribute('data-pika-day')));
                    if (opts.bound) {
                        sto(function() {
                            self.hide();
                            if (opts.field) {
                                opts.field.blur();
                            }
                        }, 100);
                    }
                }
                else if (hasClass(target, 'pika-prev')) {
                    self.prevMonth();
                }
                else if (hasClass(target, 'pika-next')) {
                    self.nextMonth();
                }
            }
            if (!hasClass(target, 'pika-select')) {
                // if this is touch event prevent mouse events emulation
                if (e.preventDefault) {
                    e.preventDefault();
                } else {
                    e.returnValue = false;
                    return false;
                }
            } else {
                self._c = true;
            }
        };

        self._onChange = function(e)
        {
            e = e || window.event;
            var target = e.target || e.srcElement;
            if (!target) {
                return;
            }
            if (hasClass(target, 'pika-select-month')) {
                self.gotoMonth(target.value);
            }
            else if (hasClass(target, 'pika-select-year')) {
                self.gotoYear(target.value);
            }
        };

        self._onInputChange = function(e)
        {
            var date;

            if (e.firedBy === self) {
                return;
            }
            if (hasMoment) {
                date = moment(opts.field.value, opts.format);
                date = (date && date.isValid()) ? date.toDate() : null;
            }
            else {
                date = new Date(Date.parse(opts.field.value));
            }
            if (isDate(date)) {
              self.setDate(date);
            }
            if (!self._v) {
                self.show();
            }
        };

        self._onInputFocus = function()
        {
            self.show();
        };

        self._onInputClick = function()
        {
            self.show();
        };

        self._onInputBlur = function()
        {
            // IE allows pika div to gain focus; catch blur the input field
            var pEl = document.activeElement;
            do {
                if (hasClass(pEl, 'pika-single')) {
                    return;
                }
            }
            while ((pEl = pEl.parentNode));

            if (!self._c) {
                self._b = sto(function() {
                    self.hide();
                }, 50);
            }
            self._c = false;
        };

        self._onClick = function(e)
        {
            e = e || window.event;
            var target = e.target || e.srcElement,
                pEl = target;
            if (!target) {
                return;
            }
            if (!hasEventListeners && hasClass(target, 'pika-select')) {
                if (!target.onchange) {
                    target.setAttribute('onchange', 'return;');
                    addEvent(target, 'change', self._onChange);
                }
            }
            do {
                if (hasClass(pEl, 'pika-single') || pEl === opts.trigger) {
                    return;
                }
            }
            while ((pEl = pEl.parentNode));
            if (self._v && target !== opts.trigger && pEl !== opts.trigger) {
                self.hide();
            }
        };

        self.el = document.createElement('div');
        self.el.className = 'pika-single' + (opts.isRTL ? ' is-rtl' : '') + (opts.theme ? ' ' + opts.theme : '');

        addEvent(self.el, 'mousedown', self._onMouseDown, true);
        addEvent(self.el, 'touchend', self._onMouseDown, true);
        addEvent(self.el, 'change', self._onChange);

        if (opts.field) {
            if (opts.container) {
                opts.container.appendChild(self.el);
            } else if (opts.bound) {
                document.body.appendChild(self.el);
            } else {
                opts.field.parentNode.insertBefore(self.el, opts.field.nextSibling);
            }
            addEvent(opts.field, 'change', self._onInputChange);

            if (!opts.defaultDate) {
                if (hasMoment && opts.field.value) {
                    opts.defaultDate = moment(opts.field.value, opts.format).toDate();
                } else {
                    opts.defaultDate = new Date(Date.parse(opts.field.value));
                }
                opts.setDefaultDate = true;
            }
        }

        var defDate = opts.defaultDate;

        if (isDate(defDate)) {
            if (opts.setDefaultDate) {
                self.setDate(defDate, true);
            } else {
                self.gotoDate(defDate);
            }
        } else {
            self.gotoDate(new Date());
        }

        if (opts.bound) {
            this.hide();
            self.el.className += ' is-bound';
            addEvent(opts.trigger, 'click', self._onInputClick);
            addEvent(opts.trigger, 'focus', self._onInputFocus);
            addEvent(opts.trigger, 'blur', self._onInputBlur);
        } else {
            this.show();
        }
    };


    /**
     * public Pikaday API
     */
    Pikaday.prototype = {


        /**
         * configure functionality
         */
        config: function(options)
        {
            if (!this._o) {
                this._o = extend({}, defaults, true);
            }

            var opts = extend(this._o, options, true);

            opts.isRTL = !!opts.isRTL;

            opts.field = (opts.field && opts.field.nodeName) ? opts.field : null;

            opts.theme = (typeof opts.theme) === 'string' && opts.theme ? opts.theme : null;

            opts.bound = !!(opts.bound !== undefined ? opts.field && opts.bound : opts.field);

            opts.trigger = (opts.trigger && opts.trigger.nodeName) ? opts.trigger : opts.field;

            opts.disableWeekends = !!opts.disableWeekends;

            opts.disableDayFn = (typeof opts.disableDayFn) === 'function' ? opts.disableDayFn : null;

            var nom = parseInt(opts.numberOfMonths, 10) || 1;
            opts.numberOfMonths = nom > 4 ? 4 : nom;

            if (!isDate(opts.minDate)) {
                opts.minDate = false;
            }
            if (!isDate(opts.maxDate)) {
                opts.maxDate = false;
            }
            if ((opts.minDate && opts.maxDate) && opts.maxDate < opts.minDate) {
                opts.maxDate = opts.minDate = false;
            }
            if (opts.minDate) {
                this.setMinDate(opts.minDate);
            }
            if (opts.maxDate) {
                this.setMaxDate(opts.maxDate);
            }

            if (isArray(opts.yearRange)) {
                var fallback = new Date().getFullYear() - 10;
                opts.yearRange[0] = parseInt(opts.yearRange[0], 10) || fallback;
                opts.yearRange[1] = parseInt(opts.yearRange[1], 10) || fallback;
            } else {
                opts.yearRange = Math.abs(parseInt(opts.yearRange, 10)) || defaults.yearRange;
                if (opts.yearRange > 100) {
                    opts.yearRange = 100;
                }
            }

            return opts;
        },

        /**
         * return a formatted string of the current selection (using Moment.js if available)
         */
        toString: function(format)
        {
            return !isDate(this._d) ? '' : hasMoment ? moment(this._d).format(format || this._o.format) : this._d.toDateString();
        },

        /**
         * return a Moment.js object of the current selection (if available)
         */
        getMoment: function()
        {
            return hasMoment ? moment(this._d) : null;
        },

        /**
         * set the current selection from a Moment.js object (if available)
         */
        setMoment: function(date, preventOnSelect)
        {
            if (hasMoment && moment.isMoment(date)) {
                this.setDate(date.toDate(), preventOnSelect);
            }
        },

        /**
         * return a Date object of the current selection
         */
        getDate: function()
        {
            return isDate(this._d) ? new Date(this._d.getTime()) : null;
        },

        /**
         * set the current selection
         */
        setDate: function(date, preventOnSelect)
        {
            if (!date) {
                this._d = null;

                if (this._o.field) {
                    this._o.field.value = '';
                    fireEvent(this._o.field, 'change', { firedBy: this });
                }

                return this.draw();
            }
            if (typeof date === 'string') {
                date = new Date(Date.parse(date));
            }
            if (!isDate(date)) {
                return;
            }

            var min = this._o.minDate,
                max = this._o.maxDate;

            if (isDate(min) && date < min) {
                date = min;
            } else if (isDate(max) && date > max) {
                date = max;
            }

            this._d = new Date(date.getTime());
            setToStartOfDay(this._d);
            this.gotoDate(this._d);

            if (this._o.field) {
                this._o.field.value = this.toString();
                fireEvent(this._o.field, 'change', { firedBy: this });
            }
            if (!preventOnSelect && typeof this._o.onSelect === 'function') {
                this._o.onSelect.call(this, this.getDate());
            }
        },

        /**
         * change view to a specific date
         */
        gotoDate: function(date)
        {
            var newCalendar = true;

            if (!isDate(date)) {
                return;
            }

            if (this.calendars) {
                var firstVisibleDate = new Date(this.calendars[0].year, this.calendars[0].month, 1),
                    lastVisibleDate = new Date(this.calendars[this.calendars.length-1].year, this.calendars[this.calendars.length-1].month, 1),
                    visibleDate = date.getTime();
                // get the end of the month
                lastVisibleDate.setMonth(lastVisibleDate.getMonth()+1);
                lastVisibleDate.setDate(lastVisibleDate.getDate()-1);
                newCalendar = (visibleDate < firstVisibleDate.getTime() || lastVisibleDate.getTime() < visibleDate);
            }

            if (newCalendar) {
                this.calendars = [{
                    month: date.getMonth(),
                    year: date.getFullYear()
                }];
                if (this._o.mainCalendar === 'right') {
                    this.calendars[0].month += 1 - this._o.numberOfMonths;
                }
            }

            this.adjustCalendars();
        },

        adjustCalendars: function() {
            this.calendars[0] = adjustCalendar(this.calendars[0]);
            for (var c = 1; c < this._o.numberOfMonths; c++) {
                this.calendars[c] = adjustCalendar({
                    month: this.calendars[0].month + c,
                    year: this.calendars[0].year
                });
            }
            this.draw();
        },

        gotoToday: function()
        {
            this.gotoDate(new Date());
        },

        /**
         * change view to a specific month (zero-index, e.g. 0: January)
         */
        gotoMonth: function(month)
        {
            if (!isNaN(month)) {
                this.calendars[0].month = parseInt(month, 10);
                this.adjustCalendars();
            }
        },

        nextMonth: function()
        {
            this.calendars[0].month++;
            this.adjustCalendars();
        },

        prevMonth: function()
        {
            this.calendars[0].month--;
            this.adjustCalendars();
        },

        /**
         * change view to a specific full year (e.g. "2012")
         */
        gotoYear: function(year)
        {
            if (!isNaN(year)) {
                this.calendars[0].year = parseInt(year, 10);
                this.adjustCalendars();
            }
        },

        /**
         * change the minDate
         */
        setMinDate: function(value)
        {
            setToStartOfDay(value);
            this._o.minDate = value;
            this._o.minYear  = value.getFullYear();
            this._o.minMonth = value.getMonth();
            this.draw();
        },

        /**
         * change the maxDate
         */
        setMaxDate: function(value)
        {
            setToStartOfDay(value);
            this._o.maxDate = value;
            this._o.maxYear = value.getFullYear();
            this._o.maxMonth = value.getMonth();
            this.draw();
        },

        setStartRange: function(value)
        {
            this._o.startRange = value;
        },

        setEndRange: function(value)
        {
            this._o.endRange = value;
        },

        /**
         * refresh the HTML
         */
        draw: function(force)
        {
            if (!this._v && !force) {
                return;
            }
            var opts = this._o,
                minYear = opts.minYear,
                maxYear = opts.maxYear,
                minMonth = opts.minMonth,
                maxMonth = opts.maxMonth,
                html = '';

            if (this._y <= minYear) {
                this._y = minYear;
                if (!isNaN(minMonth) && this._m < minMonth) {
                    this._m = minMonth;
                }
            }
            if (this._y >= maxYear) {
                this._y = maxYear;
                if (!isNaN(maxMonth) && this._m > maxMonth) {
                    this._m = maxMonth;
                }
            }

            for (var c = 0; c < opts.numberOfMonths; c++) {
                html += '<div class="pika-lendar">' + renderTitle(this, c, this.calendars[c].year, this.calendars[c].month, this.calendars[0].year) + this.render(this.calendars[c].year, this.calendars[c].month) + '</div>';
            }

            this.el.innerHTML = html;

            if (opts.bound) {
                if(opts.field.type !== 'hidden') {
                    sto(function() {
                        opts.trigger.focus();
                    }, 1);
                }
            }

            if (typeof this._o.onDraw === 'function') {
                var self = this;
                sto(function() {
                    self._o.onDraw.call(self);
                }, 0);
            }
        },

        adjustPosition: function()
        {
            var field, pEl, width, height, viewportWidth, viewportHeight, scrollTop, left, top, clientRect;

            if (this._o.container) return;

            this.el.style.position = 'absolute';

            field = this._o.trigger;
            pEl = field;
            width = this.el.offsetWidth;
            height = this.el.offsetHeight;
            viewportWidth = window.innerWidth || document.documentElement.clientWidth;
            viewportHeight = window.innerHeight || document.documentElement.clientHeight;
            scrollTop = window.pageYOffset || document.body.scrollTop || document.documentElement.scrollTop;

            if (typeof field.getBoundingClientRect === 'function') {
                clientRect = field.getBoundingClientRect();
                left = clientRect.left + window.pageXOffset;
                top = clientRect.bottom + window.pageYOffset;
            } else {
                left = pEl.offsetLeft;
                top  = pEl.offsetTop + pEl.offsetHeight;
                while((pEl = pEl.offsetParent)) {
                    left += pEl.offsetLeft;
                    top  += pEl.offsetTop;
                }
            }

            // default position is bottom & left
            if ((this._o.reposition && left + width > viewportWidth) ||
                (
                    this._o.position.indexOf('right') > -1 &&
                    left - width + field.offsetWidth > 0
                )
            ) {
                left = left - width + field.offsetWidth;
            }
            if ((this._o.reposition && top + height > viewportHeight + scrollTop) ||
                (
                    this._o.position.indexOf('top') > -1 &&
                    top - height - field.offsetHeight > 0
                )
            ) {
                top = top - height - field.offsetHeight;
            }

            this.el.style.left = left + 'px';
            this.el.style.top = top + 'px';
        },

        /**
         * render HTML for a particular month
         */
        render: function(year, month)
        {
            var opts   = this._o,
                now    = new Date(),
                days   = getDaysInMonth(year, month),
                before = new Date(year, month, 1).getDay(),
                data   = [],
                row    = [];
            setToStartOfDay(now);
            if (opts.firstDay > 0) {
                before -= opts.firstDay;
                if (before < 0) {
                    before += 7;
                }
            }
            var cells = days + before,
                after = cells;
            while(after > 7) {
                after -= 7;
            }
            cells += 7 - after;
            for (var i = 0, r = 0; i < cells; i++)
            {
                var day = new Date(year, month, 1 + (i - before)),
                    isSelected = isDate(this._d) ? compareDates(day, this._d) : false,
                    isToday = compareDates(day, now),
                    isEmpty = i < before || i >= (days + before),
                    isStartRange = opts.startRange && compareDates(opts.startRange, day),
                    isEndRange = opts.endRange && compareDates(opts.endRange, day),
                    isInRange = opts.startRange && opts.endRange && opts.startRange < day && day < opts.endRange,
                    isDisabled = (opts.minDate && day < opts.minDate) ||
                                 (opts.maxDate && day > opts.maxDate) ||
                                 (opts.disableWeekends && isWeekend(day)) ||
                                 (opts.disableDayFn && opts.disableDayFn(day)),
                    dayConfig = {
                        day: 1 + (i - before),
                        month: month,
                        year: year,
                        isSelected: isSelected,
                        isToday: isToday,
                        isDisabled: isDisabled,
                        isEmpty: isEmpty,
                        isStartRange: isStartRange,
                        isEndRange: isEndRange,
                        isInRange: isInRange
                    };

                row.push(renderDay(dayConfig));

                if (++r === 7) {
                    if (opts.showWeekNumber) {
                        row.unshift(renderWeek(i - before, month, year));
                    }
                    data.push(renderRow(row, opts.isRTL));
                    row = [];
                    r = 0;
                }
            }
            return renderTable(opts, data);
        },

        isVisible: function()
        {
            return this._v;
        },

        show: function()
        {
            if (!this._v) {
                removeClass(this.el, 'is-hidden');
                this._v = true;
                this.draw();
                if (this._o.bound) {
                    addEvent(document, 'click', this._onClick);
                    this.adjustPosition();
                }
                if (typeof this._o.onOpen === 'function') {
                    this._o.onOpen.call(this);
                }
            }
        },

        hide: function()
        {
            var v = this._v;
            if (v !== false) {
                if (this._o.bound) {
                    removeEvent(document, 'click', this._onClick);
                }
                this.el.style.position = 'static'; // reset
                this.el.style.left = 'auto';
                this.el.style.top = 'auto';
                addClass(this.el, 'is-hidden');
                this._v = false;
                if (v !== undefined && typeof this._o.onClose === 'function') {
                    this._o.onClose.call(this);
                }
            }
        },

        /**
         * GAME OVER
         */
        destroy: function()
        {
            this.hide();
            removeEvent(this.el, 'mousedown', this._onMouseDown, true);
            removeEvent(this.el, 'touchend', this._onMouseDown, true);
            removeEvent(this.el, 'change', this._onChange);
            if (this._o.field) {
                removeEvent(this._o.field, 'change', this._onInputChange);
                if (this._o.bound) {
                    removeEvent(this._o.trigger, 'click', this._onInputClick);
                    removeEvent(this._o.trigger, 'focus', this._onInputFocus);
                    removeEvent(this._o.trigger, 'blur', this._onInputBlur);
                }
            }
            if (this.el.parentNode) {
                this.el.parentNode.removeChild(this.el);
            }
        }

    };

    return Pikaday;

}));

!function(a,b){"function"==typeof define&&define.amd?define([],function(){return a.returnExportsGlobal=b()}):"object"==typeof exports?module.exports=b():a.Formatter=b()}(this,function(){var a=function(){var a={},b=4,c=new RegExp("{{([^}]+)}}","g"),d=function(a){for(var b,d=[];b=c.exec(a);)d.push(b);return d};return a.parse=function(a){var c={inpts:{},chars:{}},e=d(a),f=a.length,g=0,h=0,i=0,j=function(a){for(var d=a.length,e=0;d>e;e++)c.inpts[h]=a.charAt(e),h++;g++,i+=a.length+b-1};for(i;f>i;i++)g<e.length&&i===e[g].index?j(e[g][1]):c.chars[i-g*b]=a.charAt(i);return c.mLength=i-g*b,c},a}(),b=function(){{var a={};"undefined"!=typeof navigator?navigator.userAgent:null}return a.extend=function(a){for(var b=1;b<arguments.length;b++)for(var c in arguments[b])a[c]=arguments[b][c];return a},a.addChars=function(a,b,c){return a.substr(0,c)+b+a.substr(c,a.length)},a.removeChars=function(a,b,c){return a.substr(0,b)+a.substr(c,a.length)},a.isBetween=function(a,b){return b.sort(function(a,b){return a-b}),a>b[0]&&a<b[1]},a.addListener=function(a,b,c){return"undefined"!=typeof a.addEventListener?a.addEventListener(b,c,!1):a.attachEvent("on"+b,c)},a.preventDefault=function(a){return a.preventDefault?a.preventDefault():a.returnValue=!1},a.getClip=function(a){return a.clipboardData?a.clipboardData.getData("Text"):window.clipboardData?window.clipboardData.getData("Text"):void 0},a.getMatchingKey=function(a,b,c){for(var d in c){var e=c[d];if(a===e.which&&b===e.keyCode)return d}},a.isDelKeyDown=function(b,c){var d={backspace:{which:8,keyCode:8},"delete":{which:46,keyCode:46}};return a.getMatchingKey(b,c,d)},a.isDelKeyPress=function(b,c){var d={backspace:{which:8,keyCode:8,shiftKey:!1},"delete":{which:0,keyCode:46}};return a.getMatchingKey(b,c,d)},a.isSpecialKeyPress=function(b,c){var d={tab:{which:0,keyCode:9},enter:{which:13,keyCode:13},end:{which:0,keyCode:35},home:{which:0,keyCode:36},leftarrow:{which:0,keyCode:37},uparrow:{which:0,keyCode:38},rightarrow:{which:0,keyCode:39},downarrow:{which:0,keyCode:40},F5:{which:116,keyCode:116}};return a.getMatchingKey(b,c,d)},a.isModifier=function(a){return a.ctrlKey||a.altKey||a.metaKey},a.forEach=function(a,b,c){if(a.hasOwnProperty("length"))for(var d=0,e=a.length;e>d&&b.call(c,a[d],d,a)!==!1;d++);else for(var f in a)if(a.hasOwnProperty(f)&&b.call(c,a[f],f,a)===!1)break},a}(),c=function(a,b){function c(c){var e=[],f=[];b.forEach(c,function(c){b.forEach(c,function(b,c){var g=a.parse(b),h=d(c);return e.push(h),f.push(g),!1})});var g=function(a){var c;return b.forEach(e,function(b,d){return b.test(a)?(c=d,!1):void 0}),void 0===c?null:f[c]};return{getPattern:g,patterns:f,matchers:e}}var d=function(a){return"*"===a?/.*/:new RegExp(a)};return c}(a,b),d=function(){var a={};return a.get=function(a){if("number"==typeof a.selectionStart)return{begin:a.selectionStart,end:a.selectionEnd};var b=document.selection.createRange();if(b&&b.parentElement()===a){var c=a.createTextRange(),d=a.createTextRange(),e=a.value.length;return c.moveToBookmark(b.getBookmark()),d.collapse(!1),c.compareEndPoints("StartToEnd",d)>-1?{begin:e,end:e}:{begin:-c.moveStart("character",-e),end:-c.moveEnd("character",-e)}}return{begin:0,end:0}},a.set=function(a,b){if("object"!=typeof b&&(b={begin:b,end:b}),a.setSelectionRange)a.focus(),a.setSelectionRange(b.begin,b.end);else if(a.createTextRange){var c=a.createTextRange();c.collapse(!0),c.moveEnd("character",b.end),c.moveStart("character",b.begin),c.select()}},a}(),e=function(a,b,c){function d(b,d){var f=this;if(f.el=b,!f.el)throw new TypeError("Must provide an existing element");if(f.opts=c.extend({},e,d),"undefined"!=typeof f.opts.pattern&&(f.opts.patterns=f._specFromSinglePattern(f.opts.pattern),delete f.opts.pattern),"undefined"==typeof f.opts.patterns)throw new TypeError("Must provide a pattern or array of patterns");f.patternMatcher=a(f.opts.patterns),f._updatePattern(),f.hldrs={},f.focus=0,c.addListener(f.el,"keydown",function(a){f._keyDown(a)}),c.addListener(f.el,"keypress",function(a){f._keyPress(a)}),c.addListener(f.el,"paste",function(a){f._paste(a)}),f.opts.persistent&&(f._processKey("",!1),f.el.blur(),c.addListener(f.el,"focus",function(a){f._focus(a)}),c.addListener(f.el,"click",function(a){f._focus(a)}),c.addListener(f.el,"touchstart",function(a){f._focus(a)}))}var e={persistent:!1,repeat:!1,placeholder:" "},f={9:/[0-9]/,a:/[A-Za-z]/,"*":/[A-Za-z0-9]/};return d.addInptType=function(a,b){f[a]=b},d.prototype.resetPattern=function(c){this.opts.patterns=c?this._specFromSinglePattern(c):this.opts.patterns,this.sel=b.get(this.el),this.val=this.el.value,this.delta=0,this._removeChars(),this.patternMatcher=a(this.opts.patterns);var d=this.patternMatcher.getPattern(this.val);this.mLength=d.mLength,this.chars=d.chars,this.inpts=d.inpts,this._processKey("",!1,!0)},d.prototype._updatePattern=function(){var a=this.patternMatcher.getPattern(this.val);a&&(this.mLength=a.mLength,this.chars=a.chars,this.inpts=a.inpts)},d.prototype._keyDown=function(a){var b=a.which||a.keyCode;return b&&c.isDelKeyDown(a.which,a.keyCode)?(this._processKey(null,b),c.preventDefault(a)):void 0},d.prototype._keyPress=function(a){var b,d;return b=a.which||a.keyCode,d=c.isSpecialKeyPress(a.which,a.keyCode),c.isDelKeyPress(a.which,a.keyCode)||d||c.isModifier(a)?void 0:(this._processKey(String.fromCharCode(b),!1),c.preventDefault(a))},d.prototype._paste=function(a){return this._processKey(c.getClip(a),!1),c.preventDefault(a)},d.prototype._focus=function(){var a=this;setTimeout(function(){var c=b.get(a.el),d=c.end>a.focus,e=0===c.end;(d||e)&&b.set(a.el,a.focus)},0)},d.prototype._processKey=function(a,d,e){if(this.sel=b.get(this.el),this.val=this.el.value,this.delta=0,this.sel.begin!==this.sel.end)this.delta=-1*Math.abs(this.sel.begin-this.sel.end),this.val=c.removeChars(this.val,this.sel.begin,this.sel.end);else if(d&&46===d)this._delete();else if(d&&this.sel.begin-1>=0)this.val=c.removeChars(this.val,this.sel.end-1,this.sel.end),this.delta-=1;else if(d)return!0;d||(this.val=c.addChars(this.val,a,this.sel.begin),this.delta+=a.length),this._formatValue(e)},d.prototype._delete=function(){for(;this.chars[this.sel.begin];)this._nextPos();this.sel.begin<this.val.length&&(this._nextPos(),this.val=c.removeChars(this.val,this.sel.end-1,this.sel.end),this.delta=-1)},d.prototype._nextPos=function(){this.sel.end++,this.sel.begin++},d.prototype._formatValue=function(a){this.newPos=this.sel.end+this.delta,this._removeChars(),this._updatePattern(),this._validateInpts(),this._addChars(),this.el.value=this.val.substr(0,this.mLength),("undefined"==typeof a||a===!1)&&b.set(this.el,this.newPos)},d.prototype._removeChars=function(){this.sel.end>this.focus&&(this.delta+=this.sel.end-this.focus);for(var a=0,b=0;b<=this.mLength;b++){var d,e=this.chars[b],f=this.hldrs[b],g=b+a;g=b>=this.sel.begin?g+this.delta:g,d=this.val.charAt(g),(e&&e===d||f&&f===d)&&(this.val=c.removeChars(this.val,g,g+1),a--)}this.hldrs={},this.focus=this.val.length},d.prototype._validateInpts=function(){for(var a=0;a<this.val.length;a++){var b=this.inpts[a],d=!f[b],e=!d&&!f[b].test(this.val.charAt(a)),g=this.inpts[a];(d||e)&&g&&(this.val=c.removeChars(this.val,a,a+1),this.focusStart--,this.newPos--,this.delta--,a--)}},d.prototype._addChars=function(){if(this.opts.persistent){for(var a=0;a<=this.mLength;a++)this.val.charAt(a)||(this.val=c.addChars(this.val,this.opts.placeholder,a),this.hldrs[a]=this.opts.placeholder),this._addChar(a);for(;this.chars[this.focus];)this.focus++}else for(var b=0;b<=this.val.length;b++){if(this.delta<=0&&b===this.focus)return!0;this._addChar(b)}},d.prototype._addChar=function(a){var b=this.chars[a];return b?(c.isBetween(a,[this.sel.begin-1,this.newPos+1])&&(this.newPos++,this.delta++),a<=this.focus&&this.focus++,this.hldrs[a]&&(delete this.hldrs[a],this.hldrs[a+1]=this.opts.placeholder),void(this.val=c.addChars(this.val,b,a))):!0},d.prototype._specFromSinglePattern=function(a){return[{"*":a}]},d}(c,d,b);return e});
(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['exports'], factory);
    } else if (typeof exports === 'object' && typeof exports.nodeName !== 'string') {
        // CommonJS
        factory(exports);
    } else {
        // Browser globals
        factory(root.IBAN = {});
    }
}(this, function(exports){

    // Array.prototype.map polyfill
    // code from https://developer.mozilla.org/en-US/docs/JavaScript/Reference/Global_Objects/Array/map
    if (!Array.prototype.map){
        Array.prototype.map = function(fun /*, thisArg */){
            "use strict";

            if (this === void 0 || this === null)
                throw new TypeError();

            var t = Object(this);
            var len = t.length >>> 0;
            if (typeof fun !== "function")
                throw new TypeError();

            var res = new Array(len);
            var thisArg = arguments.length >= 2 ? arguments[1] : void 0;
            for (var i = 0; i < len; i++)
            {
                // NOTE: Absolute correctness would demand Object.defineProperty
                //       be used.  But this method is fairly new, and failure is
                //       possible only if Object.prototype or Array.prototype
                //       has a property |i| (very unlikely), so use a less-correct
                //       but more portable alternative.
                if (i in t)
                    res[i] = fun.call(thisArg, t[i], i, t);
            }

            return res;
        };
    }

    var A = 'A'.charCodeAt(0),
        Z = 'Z'.charCodeAt(0);

    /**
     * Prepare an IBAN for mod 97 computation by moving the first 4 chars to the end and transforming the letters to
     * numbers (A = 10, B = 11, ..., Z = 35), as specified in ISO13616.
     *
     * @param {string} iban the IBAN
     * @returns {string} the prepared IBAN
     */
    function iso13616Prepare(iban) {
        iban = iban.toUpperCase();
        iban = iban.substr(4) + iban.substr(0,4);

        return iban.split('').map(function(n){
            var code = n.charCodeAt(0);
            if (code >= A && code <= Z){
                // A = 10, B = 11, ... Z = 35
                return code - A + 10;
            } else {
                return n;
            }
        }).join('');
    }

    /**
     * Calculates the MOD 97 10 of the passed IBAN as specified in ISO7064.
     *
     * @param iban
     * @returns {number}
     */
    function iso7064Mod97_10(iban) {
        var remainder = iban,
            block;

        while (remainder.length > 2){
            block = remainder.slice(0, 9);
            remainder = parseInt(block, 10) % 97 + remainder.slice(block.length);
        }

        return parseInt(remainder, 10) % 97;
    }

    /**
     * Parse the BBAN structure used to configure each IBAN Specification and returns a matching regular expression.
     * A structure is composed of blocks of 3 characters (one letter and 2 digits). Each block represents
     * a logical group in the typical representation of the BBAN. For each group, the letter indicates which characters
     * are allowed in this group and the following 2-digits number tells the length of the group.
     *
     * @param {string} structure the structure to parse
     * @returns {RegExp}
     */
    function parseStructure(structure){
        // split in blocks of 3 chars
        var regex = structure.match(/(.{3})/g).map(function(block){

            // parse each structure block (1-char + 2-digits)
            var format,
                pattern = block.slice(0, 1),
                repeats = parseInt(block.slice(1), 10);

            switch (pattern){
                case "A": format = "0-9A-Za-z"; break;
                case "B": format = "0-9A-Z"; break;
                case "C": format = "A-Za-z"; break;
                case "F": format = "0-9"; break;
                case "L": format = "a-z"; break;
                case "U": format = "A-Z"; break;
                case "W": format = "0-9a-z"; break;
            }

            return '([' + format + ']{' + repeats + '})';
        });

        return new RegExp('^' + regex.join('') + '$');
    }

    /**
     * Create a new Specification for a valid IBAN number.
     *
     * @param countryCode the code of the country
     * @param length the length of the IBAN
     * @param structure the structure of the underlying BBAN (for validation and formatting)
     * @param example an example valid IBAN
     * @constructor
     */
    function Specification(countryCode, length, structure, example){

        this.countryCode = countryCode;
        this.length = length;
        this.structure = structure;
        this.example = example;
    }

    /**
     * Lazy-loaded regex (parse the structure and construct the regular expression the first time we need it for validation)
     */
    Specification.prototype._regex = function(){
        return this._cachedRegex || (this._cachedRegex = parseStructure(this.structure))
    };

    /**
     * Check if the passed iban is valid according to this specification.
     *
     * @param {String} iban the iban to validate
     * @returns {boolean} true if valid, false otherwise
     */
    Specification.prototype.isValid = function(iban){
        return this.length == iban.length
            && this.countryCode === iban.slice(0,2)
            && this._regex().test(iban.slice(4))
            && iso7064Mod97_10(iso13616Prepare(iban)) == 1;
    };

    /**
     * Convert the passed IBAN to a country-specific BBAN.
     *
     * @param iban the IBAN to convert
     * @param separator the separator to use between BBAN blocks
     * @returns {string} the BBAN
     */
    Specification.prototype.toBBAN = function(iban, separator) {
        return this._regex().exec(iban.slice(4)).slice(1).join(separator);
    };

    /**
     * Convert the passed BBAN to an IBAN for this country specification.
     * Please note that <i>"generation of the IBAN shall be the exclusive responsibility of the bank/branch servicing the account"</i>.
     * This method implements the preferred algorithm described in http://en.wikipedia.org/wiki/International_Bank_Account_Number#Generating_IBAN_check_digits
     *
     * @param bban the BBAN to convert to IBAN
     * @returns {string} the IBAN
     */
    Specification.prototype.fromBBAN = function(bban) {
        if (!this.isValidBBAN(bban)){
            throw new Error('Invalid BBAN');
        }

        var remainder = iso7064Mod97_10(iso13616Prepare(this.countryCode + '00' + bban)),
            checkDigit = ('0' + (98 - remainder)).slice(-2);

        return this.countryCode + checkDigit + bban;
    };

    /**
     * Check of the passed BBAN is valid.
     * This function only checks the format of the BBAN (length and matching the letetr/number specs) but does not
     * verify the check digit.
     *
     * @param bban the BBAN to validate
     * @returns {boolean} true if the passed bban is a valid BBAN according to this specification, false otherwise
     */
    Specification.prototype.isValidBBAN = function(bban) {
        return this.length - 4 == bban.length
            && this._regex().test(bban);
    };

    var countries = {};

    function addSpecification(IBAN){
        countries[IBAN.countryCode] = IBAN;
    }

    addSpecification(new Specification("AD", 24, "F04F04A12",          "AD1200012030200359100100"));
    addSpecification(new Specification("AE", 23, "F03F16",             "AE070331234567890123456"));
    addSpecification(new Specification("AL", 28, "F08A16",             "AL47212110090000000235698741"));
    addSpecification(new Specification("AT", 20, "F05F11",             "AT611904300234573201"));
    addSpecification(new Specification("AZ", 28, "U04A20",             "AZ21NABZ00000000137010001944"));
    addSpecification(new Specification("BA", 20, "F03F03F08F02",       "BA391290079401028494"));
    addSpecification(new Specification("BE", 16, "F03F07F02",          "BE68539007547034"));
    addSpecification(new Specification("BG", 22, "U04F04F02A08",       "BG80BNBG96611020345678"));
    addSpecification(new Specification("BH", 22, "U04A14",             "BH67BMAG00001299123456"));
    addSpecification(new Specification("BR", 29, "F08F05F10U01A01",    "BR9700360305000010009795493P1"));
    addSpecification(new Specification("CH", 21, "F05A12",             "CH9300762011623852957"));
    addSpecification(new Specification("CR", 21, "F03F14",             "CR0515202001026284066"));
    addSpecification(new Specification("CY", 28, "F03F05A16",          "CY17002001280000001200527600"));
    addSpecification(new Specification("CZ", 24, "F04F06F10",          "CZ6508000000192000145399"));
    addSpecification(new Specification("DE", 22, "F08F10",             "DE89370400440532013000"));
    addSpecification(new Specification("DK", 18, "F04F09F01",          "DK5000400440116243"));
    addSpecification(new Specification("DO", 28, "U04F20",             "DO28BAGR00000001212453611324"));
    addSpecification(new Specification("EE", 20, "F02F02F11F01",       "EE382200221020145685"));
    addSpecification(new Specification("ES", 24, "F04F04F01F01F10",    "ES9121000418450200051332"));
    addSpecification(new Specification("FI", 18, "F06F07F01",          "FI2112345600000785"));
    addSpecification(new Specification("FO", 18, "F04F09F01",          "FO6264600001631634"));
    addSpecification(new Specification("FR", 27, "F05F05A11F02",       "FR1420041010050500013M02606"));
    addSpecification(new Specification("GB", 22, "U04F06F08",          "GB29NWBK60161331926819"));
    addSpecification(new Specification("GE", 22, "U02F16",             "GE29NB0000000101904917"));
    addSpecification(new Specification("GI", 23, "U04A15",             "GI75NWBK000000007099453"));
    addSpecification(new Specification("GL", 18, "F04F09F01",          "GL8964710001000206"));
    addSpecification(new Specification("GR", 27, "F03F04A16",          "GR1601101250000000012300695"));
    addSpecification(new Specification("GT", 28, "A04A20",             "GT82TRAJ01020000001210029690"));
    addSpecification(new Specification("HR", 21, "F07F10",             "HR1210010051863000160"));
    addSpecification(new Specification("HU", 28, "F03F04F01F15F01",    "HU42117730161111101800000000"));
    addSpecification(new Specification("IE", 22, "U04F06F08",          "IE29AIBK93115212345678"));
    addSpecification(new Specification("IL", 23, "F03F03F13",          "IL620108000000099999999"));
    addSpecification(new Specification("IS", 26, "F04F02F06F10",       "IS140159260076545510730339"));
    addSpecification(new Specification("IT", 27, "U01F05F05A12",       "IT60X0542811101000000123456"));
    addSpecification(new Specification("KW", 30, "U04A22",             "KW81CBKU0000000000001234560101"));
    addSpecification(new Specification("KZ", 20, "F03A13",             "KZ86125KZT5004100100"));
    addSpecification(new Specification("LB", 28, "F04A20",             "LB62099900000001001901229114"));
    addSpecification(new Specification("LC", 32, "U04F24",             "LC07HEMM000100010012001200013015"));
    addSpecification(new Specification("LI", 21, "F05A12",             "LI21088100002324013AA"));
    addSpecification(new Specification("LT", 20, "F05F11",             "LT121000011101001000"));
    addSpecification(new Specification("LU", 20, "F03A13",             "LU280019400644750000"));
    addSpecification(new Specification("LV", 21, "U04A13",             "LV80BANK0000435195001"));
    addSpecification(new Specification("MC", 27, "F05F05A11F02",       "MC5811222000010123456789030"));
    addSpecification(new Specification("MD", 24, "U02A18",             "MD24AG000225100013104168"));
    addSpecification(new Specification("ME", 22, "F03F13F02",          "ME25505000012345678951"));
    addSpecification(new Specification("MK", 19, "F03A10F02",          "MK07250120000058984"));
    addSpecification(new Specification("MR", 27, "F05F05F11F02",       "MR1300020001010000123456753"));
    addSpecification(new Specification("MT", 31, "U04F05A18",          "MT84MALT011000012345MTLCAST001S"));
    addSpecification(new Specification("MU", 30, "U04F02F02F12F03U03", "MU17BOMM0101101030300200000MUR"));
    addSpecification(new Specification("NL", 18, "U04F10",             "NL91ABNA0417164300"));
    addSpecification(new Specification("NO", 15, "F04F06F01",          "NO9386011117947"));
    addSpecification(new Specification("PK", 24, "U04A16",             "PK36SCBL0000001123456702"));
    addSpecification(new Specification("PL", 28, "F08F16",             "PL61109010140000071219812874"));
    addSpecification(new Specification("PS", 29, "U04A21",             "PS92PALS000000000400123456702"));
    addSpecification(new Specification("PT", 25, "F04F04F11F02",       "PT50000201231234567890154"));
    addSpecification(new Specification("RO", 24, "U04A16",             "RO49AAAA1B31007593840000"));
    addSpecification(new Specification("RS", 22, "F03F13F02",          "RS35260005601001611379"));
    addSpecification(new Specification("SA", 24, "F02A18",             "SA0380000000608010167519"));
    addSpecification(new Specification("SE", 24, "F03F16F01",          "SE4550000000058398257466"));
    addSpecification(new Specification("SI", 19, "F05F08F02",          "SI56263300012039086"));
    addSpecification(new Specification("SK", 24, "F04F06F10",          "SK3112000000198742637541"));
    addSpecification(new Specification("SM", 27, "U01F05F05A12",       "SM86U0322509800000000270100"));
    addSpecification(new Specification("ST", 25, "F08F11F02",          "ST68000100010051845310112"));
    addSpecification(new Specification("TL", 23, "F03F14F02",          "TL380080012345678910157"));
    addSpecification(new Specification("TN", 24, "F02F03F13F02",       "TN5910006035183598478831"));
    addSpecification(new Specification("TR", 26, "F05F01A16",          "TR330006100519786457841326"));
    addSpecification(new Specification("VG", 24, "U04F16",             "VG96VPVG0000012345678901"));
    addSpecification(new Specification("XK", 20, "F04F10F02",          "XK051212012345678906"));

    // Angola
    addSpecification(new Specification("AO", 25, "F21",                "AO69123456789012345678901"));
    // Burkina
    addSpecification(new Specification("BF", 27, "F23",                "BF2312345678901234567890123"));
    // Burundi
    addSpecification(new Specification("BI", 16, "F12",                "BI41123456789012"));
    // Benin
    addSpecification(new Specification("BJ", 28, "F24",                "BJ39123456789012345678901234"));
    // Ivory
    addSpecification(new Specification("CI", 28, "U01F23",             "CI17A12345678901234567890123"));
    // Cameron
    addSpecification(new Specification("CM", 27, "F23",                "CM9012345678901234567890123"));
    // Cape Verde
    addSpecification(new Specification("CV", 25, "F21",                "CV30123456789012345678901"));
    // Algeria
    addSpecification(new Specification("DZ", 24, "F20",                "DZ8612345678901234567890"));
    // Iran
    addSpecification(new Specification("IR", 26, "F22",                "IR861234568790123456789012"));
    // Jordan
    addSpecification(new Specification("JO", 30, "A04F22",             "JO15AAAA1234567890123456789012"));
    // Madagascar
    addSpecification(new Specification("MG", 27, "F23",                "MG1812345678901234567890123"));
    // Mali
    addSpecification(new Specification("ML", 28, "U01F23",             "ML15A12345678901234567890123"));
    // Mozambique
    addSpecification(new Specification("MZ", 25, "F21",                "MZ25123456789012345678901"));
    // Quatar
    addSpecification(new Specification("QA", 29, "U04A21",             "QA30AAAA123456789012345678901"));
    // Senegal
    addSpecification(new Specification("SN", 28, "U01F23",             "SN52A12345678901234567890123"));
    // Ukraine
    addSpecification(new Specification("UA", 29, "F25",                "UA511234567890123456789012345"));

    var NON_ALPHANUM = /[^a-zA-Z0-9]/g,
        EVERY_FOUR_CHARS =/(.{4})(?!$)/g;

    /**
     * Utility function to check if a variable is a String.
     *
     * @param v
     * @returns {boolean} true if the passed variable is a String, false otherwise.
     */
    function isString(v){
        return (typeof v == 'string' || v instanceof String);
    }

    /**
     * Check if an IBAN is valid.
     *
     * @param {String} iban the IBAN to validate.
     * @returns {boolean} true if the passed IBAN is valid, false otherwise
     */
    exports.isValid = function(iban){
        if (!isString(iban)){
            return false;
        }
        iban = this.electronicFormat(iban);
        var countryStructure = countries[iban.slice(0,2)];
        return !!countryStructure && countryStructure.isValid(iban);
    };

    /**
     * Convert an IBAN to a BBAN.
     *
     * @param iban
     * @param {String} [separator] the separator to use between the blocks of the BBAN, defaults to ' '
     * @returns {string|*}
     */
    exports.toBBAN = function(iban, separator){
        if (typeof separator == 'undefined'){
            separator = ' ';
        }
        iban = this.electronicFormat(iban);
        var countryStructure = countries[iban.slice(0,2)];
        if (!countryStructure) {
            throw new Error('No country with code ' + iban.slice(0,2));
        }
        return countryStructure.toBBAN(iban, separator);
    };

    /**
     * Convert the passed BBAN to an IBAN for this country specification.
     * Please note that <i>"generation of the IBAN shall be the exclusive responsibility of the bank/branch servicing the account"</i>.
     * This method implements the preferred algorithm described in http://en.wikipedia.org/wiki/International_Bank_Account_Number#Generating_IBAN_check_digits
     *
     * @param countryCode the country of the BBAN
     * @param bban the BBAN to convert to IBAN
     * @returns {string} the IBAN
     */
    exports.fromBBAN = function(countryCode, bban){
        var countryStructure = countries[countryCode];
        if (!countryStructure) {
            throw new Error('No country with code ' + countryCode);
        }
        return countryStructure.fromBBAN(this.electronicFormat(bban));
    };

    /**
     * Check the validity of the passed BBAN.
     *
     * @param countryCode the country of the BBAN
     * @param bban the BBAN to check the validity of
     */
    exports.isValidBBAN = function(countryCode, bban){
        if (!isString(bban)){
            return false;
        }
        var countryStructure = countries[countryCode];
        return countryStructure && countryStructure.isValidBBAN(this.electronicFormat(bban));
    };

    /**
     *
     * @param iban
     * @param separator
     * @returns {string}
     */
    exports.printFormat = function(iban, separator){
        if (typeof separator == 'undefined'){
            separator = ' ';
        }
        return this.electronicFormat(iban).replace(EVERY_FOUR_CHARS, "$1" + separator);
    };

    /**
     *
     * @param iban
     * @returns {string}
     */
    exports.electronicFormat = function(iban){
        return iban.replace(NON_ALPHANUM, '').toUpperCase();
    };

    /**
     * An object containing all the known IBAN specifications.
     */
    exports.countries = countries;

}));

/*
 * classList.js: Cross-browser full element.classList implementation.
 * 2012-11-15
 *
 * By Eli Grey, http://eligrey.com
 * Public Domain.
 * NO WARRANTY EXPRESSED OR IMPLIED. USE AT YOUR OWN RISK.
 */

/*global self, document, DOMException */

/*! @source http://purl.eligrey.com/github/classList.js/blob/master/classList.js*/
;(function() {
if (typeof document !== "undefined" && !("classList" in document.documentElement)) {

(function (view) {

    "use strict";

    if (!('HTMLElement' in view) && !('Element' in view)) return;

    var
          classListProp = "classList"
        , protoProp = "prototype"
        , elemCtrProto = (view.HTMLElement || view.Element)[protoProp]
        , objCtr = Object
        , strTrim = String[protoProp].trim || function () {
            return this.replace(/^\s+|\s+$/g, "");
        }
        , arrIndexOf = Array[protoProp].indexOf || function (item) {
            var
                  i = 0
                , len = this.length
            ;
            for (; i < len; i++) {
                if (i in this && this[i] === item) {
                    return i;
                }
            }
            return -1;
        }
        // Vendors: please allow content code to instantiate DOMExceptions
        , DOMEx = function (type, message) {
            this.name = type;
            this.code = DOMException[type];
            this.message = message;
        }
        , checkTokenAndGetIndex = function (classList, token) {
            if (token === "") {
                throw new DOMEx(
                      "SYNTAX_ERR"
                    , "An invalid or illegal string was specified"
                );
            }
            if (/\s/.test(token)) {
                throw new DOMEx(
                      "INVALID_CHARACTER_ERR"
                    , "String contains an invalid character"
                );
            }
            return arrIndexOf.call(classList, token);
        }
        , ClassList = function (elem) {
            var
                  trimmedClasses = strTrim.call(elem.className)
                , classes = trimmedClasses ? trimmedClasses.split(/\s+/) : []
                , i = 0
                , len = classes.length
            ;
            for (; i < len; i++) {
                this.push(classes[i]);
            }
            this._updateClassName = function () {
                elem.className = this.toString();
            };
        }
        , classListProto = ClassList[protoProp] = []
        , classListGetter = function () {
            return new ClassList(this);
        }
    ;
    // Most DOMException implementations don't allow calling DOMException's toString()
    // on non-DOMExceptions. Error's toString() is sufficient here.
    DOMEx[protoProp] = Error[protoProp];
    classListProto.item = function (i) {
        return this[i] || null;
    };
    classListProto.contains = function (token) {
        token += "";
        return checkTokenAndGetIndex(this, token) !== -1;
    };
    classListProto.add = function () {
        var
              tokens = arguments
            , i = 0
            , l = tokens.length
            , token
            , updated = false
        ;
        do {
            token = tokens[i] + "";
            if (checkTokenAndGetIndex(this, token) === -1) {
                this.push(token);
                updated = true;
            }
        }
        while (++i < l);

        if (updated) {
            this._updateClassName();
        }
    };
    classListProto.remove = function () {
        var
              tokens = arguments
            , i = 0
            , l = tokens.length
            , token
            , updated = false
        ;
        do {
            token = tokens[i] + "";
            var index = checkTokenAndGetIndex(this, token);
            if (index !== -1) {
                this.splice(index, 1);
                updated = true;
            }
        }
        while (++i < l);

        if (updated) {
            this._updateClassName();
        }
    };
    classListProto.toggle = function (token, forse) {
        token += "";

        var
              result = this.contains(token)
            , method = result ?
                forse !== true && "remove"
            :
                forse !== false && "add"
        ;

        if (method) {
            this[method](token);
        }

        return !result;
    };
    classListProto.toString = function () {
        return this.join(" ");
    };

    if (objCtr.defineProperty) {
        var classListPropDesc = {
              get: classListGetter
            , enumerable: true
            , configurable: true
        };
        try {
            objCtr.defineProperty(elemCtrProto, classListProp, classListPropDesc);
        } catch (ex) { // IE 8 doesn't support enumerable:true
            if (ex.number === -0x7FF5EC54) {
                classListPropDesc.enumerable = false;
                objCtr.defineProperty(elemCtrProto, classListProp, classListPropDesc);
            }
        }
    } else if (objCtr[protoProp].__defineGetter__) {
        elemCtrProto.__defineGetter__(classListProp, classListGetter);
    }

    }(self));
}
})();

/* !
 * @author Sean Coker <sean@seancoker.com>
 * @version 1.8.0
 * @url http://sean.is/poppin/tags
 * @license MIT
 * @description Taggle is a dependency-less tagging library
 */

(function(window, document) {
    'use strict';

    /////////////////////
    // Default options //
    /////////////////////

    var noop = function() {};

    var DEFAULTS = {
        /**
         * Class names to be added on each tag entered
         * @type {String}
         */
        additionalTagClasses: '',

        /**
         * Allow duplicate tags to be entered in the field?
         * @type {Boolean}
         */
        allowDuplicates: false,

        /**
         * Allow the saving of a tag on blur, rather than it being
         * removed.
         *
         * @type {Boolean}
         */
        saveOnBlur: false,

        /**
         * Class name that will be added onto duplicate existant tag
         * @type {String}
         * @todo
         * @deprecated can be handled by onBeforeTagAdd
         */
        duplicateTagClass: '',

        /**
         * Class added to the container div when focused
         * @type {String}
         */
        containerFocusClass: 'active',

        /**
         * Should the input be focused when the container is clicked?
         * @type {Bool}
         */
        focusInputOnContainerClick: true,

        /**
         * Name added to the hidden inputs within each tag
         * @type {String}
         */
        hiddenInputName: 'taggles[]',

        /**
         * Tags that should be preloaded in the div on load
         * @type {Array}
         */
        tags: [],

        /**
         * Add an ID to each of the tags.
         * @type {Boolean}
         * @todo
         * @deprecated make this the default in next version
         */
        attachTagId: false,

        /**
         * Tags that the user will be restricted to
         * @type {Array}
         */
        allowedTags: [],

        /**
         * Tags that the user will not be able to add
         * @type {Array}
         */
        disallowedTags: [],

        /**
         * Limit the number of tags that can be added
         * @type {Number}
         */
        maxTags: null,

        /**
         * If within a form, you can specify the tab index flow
         * @type {Number}
         */
        tabIndex: 1,

        /**
         * Placeholder string to be placed in an empty taggle field
         * @type {String}
         */
        placeholder: 'Enter tags...',

        /**
         * Keycodes that will add a tag
         * @type {Array}
         */
        submitKeys: [],

        /**
         * Preserve case of tags being added ie
         * "tag" is different than "Tag"
         * @type {Boolean}
         */
        preserveCase: false,

        /**
         * Function hook called with the to-be-added tag DOM element.
         * Use this function to edit the list item before it is appended
         * to the DOM
         * @param  {HTMLElement} li The list item to be added
         */
        tagFormatter: noop,

        /**
         * Function hook called before a tag is added. Return false
         * to prevent tag from being added
         * @param  {String} tag The tag to be added
         */
        onBeforeTagAdd: noop,

        /**
         * Function hook called when a tag is added
         * @param  {Event} event Event triggered when tag was added
         * @param  {String} tag The tag added
         */
        onTagAdd: noop,

        /**
         * Function hook called before a tag is removed. Return false
         * to prevent tag from being removed
         * @param  {String} tag The tag to be removed
         */
        onBeforeTagRemove: noop,

        /**
         * Function hook called when a tag is removed
         * @param  {Event} event Event triggered when tag was removed
         * @param  {String} tag The tag removed
         */
        onTagRemove: noop
    };

    var BACKSPACE = 8;
    var COMMA = 188;
    var TAB = 9;
    var ENTER = 13;

    //////////////////////
    // Helper functions //
    //////////////////////

    function _extend() {
        var master = arguments[0];
        for (var i = 1, l = arguments.length; i < l; i++) {
            var object = arguments[i];
            for (var key in object) {
                if (object.hasOwnProperty(key)) {
                    master[key] = object[key];
                }
            }
        }

        return master;
    }

    function _isArray(arr) {
        if (Array.isArray) {
            return Array.isArray(arr);
        }
        return Object.prototype.toString.call(arr) === '[object Array]';
    }

    function _on(element, eventName, handler) {
        if (element.addEventListener) {
            element.addEventListener(eventName, handler, false);
        }
        else if (element.attachEvent) {
            element.attachEvent('on' + eventName, handler);
        }
        else {
            element['on' + eventName] = handler;
        }
    }

    function _trim(str) {
        return str.replace(/^\s+|\s+$/g, '');
    }

    function _setText(el, text) {
        if (window.attachEvent && !window.addEventListener) { // <= IE8
            el.innerText = text;
        }
        else {
            el.textContent = text;
        }
    }

    /**
     * Constructor
     * @param {Mixed} el ID of an element or the actual element
     * @param {Object} options
     */
    var Taggle = function(el, options) {
        this.settings = _extend({}, DEFAULTS, options);
        this.measurements = {
            container: {
                rect: null,
                style: null,
                padding: null
            }
        };
        this.container = el;
        this.tag = {
            values: [],
            elements: []
        };
        this.list = document.createElement('ul');
        this.inputLi = document.createElement('li');
        this.input = document.createElement('input');
        this.sizer = document.createElement('div');
        this.pasting = false;
        this.placeholder = null;

        if (this.settings.placeholder) {
            this.placeholder = document.createElement('span');
        }

        if (!this.settings.submitKeys.length) {
            this.settings.submitKeys = [COMMA, TAB, ENTER];
        }

        if (typeof el === 'string') {
            this.container = document.getElementById(el);
        }

        this._id = 0;
        this._getMeasurements();
        this._setupTextarea();
        this._attachEvents();
    };

    /**
     * Gets all the layout measurements up front
     */
    Taggle.prototype._getMeasurements = function() {
        var style;
        var lpad;
        var rpad;

        this.measurements.container.rect = this.container.getBoundingClientRect();
        this.measurements.container.style = window.getComputedStyle(this.container);

        style = this.measurements.container.style;
        lpad = parseInt(style['padding-left'] || style.paddingLeft, 10);
        rpad = parseInt(style['padding-right'] || style.paddingRight, 10);

        this.measurements.container.padding = lpad + rpad;
    };

    /**
     * Setup the div container for tags to be entered
     */
    Taggle.prototype._setupTextarea = function() {
        var fontSize;

        this.list.className = 'taggle_list';
        this.input.type = 'text';
        this.input.className = 'taggle_input';
        this.input.tabIndex = this.settings.tabIndex;
        this.sizer.className = 'taggle_sizer';

        if (this.settings.tags.length) {
            for (var i = 0, len = this.settings.tags.length; i < len; i++) {
                var taggle = this._createTag(this.settings.tags[i]);
                this.list.appendChild(taggle);
            }
        }

        if (this.placeholder) {
            this.placeholder.style.opacity = 0;
            this.placeholder.classList.add('taggle_placeholder');
            this.container.appendChild(this.placeholder);
            _setText(this.placeholder, this.settings.placeholder);

            if (!this.settings.tags.length) {
                this.placeholder.style.opacity = 1;
            }
        }

        this.inputLi.appendChild(this.input);
        this.list.appendChild(this.inputLi);
        this.container.appendChild(this.list);
        this.container.appendChild(this.sizer);
        fontSize = window.getComputedStyle(this.input).fontSize;
        this.sizer.style.fontSize = fontSize;
    };

    /**
     * Attaches neccessary events
     */
    Taggle.prototype._attachEvents = function() {
        var self = this;

        if (this.settings.focusInputOnContainerClick) {
            _on(this.container, 'click', function() {
                self.input.focus();
            });
        }

        _on(this.input, 'focus', this._focusInput.bind(this));
        _on(this.input, 'blur', this._blurEvent.bind(this));
        _on(this.input, 'keydown', this._keydownEvents.bind(this));
        _on(this.input, 'keyup', this._keyupEvents.bind(this));
    };

    /**
     * Resizes the hidden input where user types to fill in the
     * width of the div
     */
    Taggle.prototype._fixInputWidth = function() {
        var width;
        var inputRect;
        var rect;
        var leftPos;
        var padding;

        // Reset width incase we've broken to the next line on a backspace erase
        this._setInputWidth();

        inputRect = this.input.getBoundingClientRect();
        rect = this.measurements.container.rect;
        width = ~~rect.width;
        // Could probably just use right - left all the time
        // but eh, this check is mostly for IE8
        if (!width) {
            width = ~~rect.right - ~~rect.left;
        }
        leftPos = ~~inputRect.left - ~~rect.left;
        padding = this.measurements.container.padding;

        this._setInputWidth(width - leftPos - padding);
    };

    /**
     * Returns whether or not the specified tag text can be added
     * @param  {Event} e event causing the potentially added tag
     * @param  {String} text tag value
     * @return {Boolean}
     */
    Taggle.prototype._canAdd = function(e, text) {
        if (!text) {
            return false;
        }
        var limit = this.settings.maxTags;
        if (limit !== null && limit <= this.getTagValues().length) {
            return false;
        }

        if (this.settings.onBeforeTagAdd(e, text) === false) {
            return false;
        }

        if (!this.settings.allowDuplicates && this._hasDupes(text)) {
            return false;
        }

        var sensitive = this.settings.preserveCase;
        var allowed = this.settings.allowedTags;

        if (allowed.length && !this._tagIsInArray(text, allowed, sensitive)) {
            return false;
        }

        var disallowed = this.settings.disallowedTags;
        if (disallowed.length && this._tagIsInArray(text, disallowed, sensitive)) {
            return false;
        }

        return true;
    };

    /**
     * Returns whether a string is in an array based on case sensitivity
     *
     * @param  {String} text string to search for
     * @param  {Array} arr array of strings to search through
     * @param  {Boolean} caseSensitive
     * @return {Boolean}
     */
    Taggle.prototype._tagIsInArray = function(text, arr, caseSensitive) {
        if (caseSensitive) {
            return arr.indexOf(text) !== -1;
        }

        var lowercased = [].slice.apply(arr).map(function(str) {
            return str.toLowerCase();
        });

        return lowercased.indexOf(text) !== -1;
    };

    /**
     * Appends tag with its corresponding input to the list
     * @param  {Event} e
     * @param  {String} text
     */
    Taggle.prototype._add = function(e, text) {
        var self = this;
        var values = text || '';

        if (typeof text !== 'string') {
            values = _trim(this.input.value);
        }

        values.split(',').map(function(val) {
            return self._formatTag(val);
        }).forEach(function(val) {
            if (!self._canAdd(e, val)) {
                return;
            }

            var li = self._createTag(val);
            var lis = self.list.children;
            var lastLi = lis[lis.length - 1];
            self.list.insertBefore(li, lastLi);


            val = self.tag.values[self.tag.values.length - 1];

            self.settings.onTagAdd(e, val);

            self.input.value = '';
            self._setInputWidth();
            self._fixInputWidth();
            self._focusInput();
        });
    };

    /**
     * Removes last tag if it has already been probed
     * @param  {Event} e
     */
    Taggle.prototype._checkLastTag = function(e) {
        e = e || window.event;

        var taggles = this.container.querySelectorAll('.taggle');
        var lastTaggle = taggles[taggles.length - 1];
        var hotClass = 'taggle_hot';
        var heldDown = this.input.classList.contains('taggle_back');

        // prevent holding backspace from deleting all tags
        if (this.input.value === '' && e.keyCode === BACKSPACE && !heldDown) {
            if (lastTaggle.classList.contains(hotClass)) {
                this.input.classList.add('taggle_back');
                this._remove(lastTaggle, e);
                this._fixInputWidth();
                this._focusInput();
            }
            else {
                lastTaggle.classList.add(hotClass);
            }
        }
        else if (lastTaggle.classList.contains(hotClass)) {
            lastTaggle.classList.remove(hotClass);
        }
    };

    /**
     * Setter for the hidden input.
     * @param {Number} width
     */
    Taggle.prototype._setInputWidth = function(width) {
        this.input.style.width = (width || 10) + 'px';
    };

    /**
     * Checks global tags array if provided tag exists
     * @param  {String} text
     * @return {Boolean}
     */
    Taggle.prototype._hasDupes = function(text) {
        var needle = this.tag.values.indexOf(text);
        var tagglelist = this.container.querySelector('.taggle_list');
        var dupes;

        if (this.settings.duplicateTagClass) {
            dupes = tagglelist.querySelectorAll('.' + this.settings.duplicateTagClass);
            for (var i = 0, len = dupes.length; i < len; i++) {
                dupes[i].classList.remove(this.settings.duplicateTagClass);
            }
        }

        // if found
        if (needle > -1) {
            if (this.settings.duplicateTagClass) {
                tagglelist.childNodes[needle].classList.add(this.settings.duplicateTagClass);
            }
            return true;
        }

        return false;
    };

    /**
     * Checks whether or not the key pressed is acceptable
     * @param  {Number}  key code
     * @return {Boolean}
     */
    Taggle.prototype._isConfirmKey = function(key) {
        var confirmKey = false;

        if (this.settings.submitKeys.indexOf(key) > -1) {
            confirmKey = true;
        }

        return confirmKey;
    };

    // Event handlers

    /**
     * Handles focus state of div container.
     */
    Taggle.prototype._focusInput = function() {
        this._fixInputWidth();

        if (!this.container.classList.contains(this.settings.containerFocusClass)) {
            this.container.classList.add(this.settings.containerFocusClass);
        }

        if (this.placeholder) {
            this.placeholder.style.opacity = 0;
        }
    };

    /**
     * Runs all the events that need to happen on a blur
     * @param  {Event} e
     */
    Taggle.prototype._blurEvent = function(e) {
        if (this.container.classList.contains(this.settings.containerFocusClass)) {
            this.container.classList.remove(this.settings.containerFocusClass);
        }

        if (!this.tag.values.length && this.placeholder) {
            this.placeholder.style.opacity = 1;
        }

        if (this.settings.saveOnBlur) {
            e = e || window.event;

            this._listenForEndOfContainer();

            if (this.input.value !== '') {
                this._confirmValidTagEvent(e);
                return;
            }

            if (this.tag.values.length) {
                this._checkLastTag(e);
            }
        }
        else {
            this.input.value = '';
            this._setInputWidth();
        }
    };

    /**
     * Runs all the events that need to run on keydown
     * @param  {Event} e
     */
    Taggle.prototype._keydownEvents = function(e) {
        e = e || window.event;

        var key = e.keyCode;
        this.pasting = false;

        this._listenForEndOfContainer();

        if (key === 86 && e.metaKey) {
            this.pasting = true;
        }

        if (this._isConfirmKey(key) && this.input.value !== '') {
            this._confirmValidTagEvent(e);
            return;
        }

        if (this.tag.values.length) {
            this._checkLastTag(e);
        }
    };

    /**
     * Runs all the events that need to run on keyup
     * @param  {Event} e
     */
    Taggle.prototype._keyupEvents = function(e) {
        e = e || window.event;

        this.input.classList.remove('taggle_back');

        _setText(this.sizer, this.input.value);

        if (this.pasting && this.input.value !== '') {
            this._add(e);
            this.pasting = false;
        }
    };

    /**
     * Confirms the inputted value to be converted to a tag
     * @param  {Event} e
     */
    Taggle.prototype._confirmValidTagEvent = function(e) {
        e = e || window.event;

        // prevents from jumping out of textarea
        if (e.preventDefault) {
            e.preventDefault();
        }
        else {
            e.returnValue = false;
        }

        this._add(e);
    };

    /**
     * Approximates when the hidden input should break to the next line
     */
    Taggle.prototype._listenForEndOfContainer = function() {
        var width = this.sizer.getBoundingClientRect().width;
        var max = this.measurements.container.rect.width - this.measurements.container.padding;
        var size = parseInt(this.sizer.style.fontSize, 10);

        // 1.5 just seems to be a good multiplier here
        if (width + (size * 1.5) > parseInt(this.input.style.width, 10)) {
            this.input.style.width = max + 'px';
        }
    };

    Taggle.prototype._createTag = function(text) {
        var li = document.createElement('li');
        var close = document.createElement('button');
        var hidden = document.createElement('input');
        var span = document.createElement('span');

        text = this._formatTag(text);

        close.innerHTML = '&times;';
        close.className = 'close';
        close.type = 'button';
        _on(close, 'click', this._remove.bind(this, close));

        _setText(span, text);
        span.className = 'taggle_text';

        li.className = 'taggle ' + this.settings.additionalTagClasses;

        hidden.type = 'hidden';
        hidden.value = text;
        hidden.name = this.settings.hiddenInputName;

        li.appendChild(span);
        li.appendChild(close);
        li.appendChild(hidden);

        var formatted = this.settings.tagFormatter(li);

        if (typeof formatted !== 'undefined') {
            li = formatted;
        }

        if (!(li instanceof HTMLElement) || li.tagName !== 'LI') {
            throw new Error('tagFormatter must return an li element');
        }

        if (this.settings.attachTagId) {
            this._id += 1;
            text = {
                text: text,
                id: this._id
            };
        }

        this.tag.values.push(text);
        this.tag.elements.push(li);

        return li;
    };

    /**
     * Removes tag from the tags collection
     * @param  {li} li List item to remove
     * @param  {Event} e
     */
    Taggle.prototype._remove = function(li, e) {
        var text;
        var elem;
        var index;

        if (li.tagName.toLowerCase() !== 'li') {
            li = li.parentNode;
        }

        elem = (li.tagName.toLowerCase() === 'a') ? li.parentNode : li;
        index = this.tag.elements.indexOf(elem);

        text = this.tag.values[index];

        if (this.settings.onBeforeTagRemove(e, text) === false) {
            return;
        }

        li.parentNode.removeChild(li);

        // Going to assume the indicies match for now
        this.tag.elements.splice(index, 1);
        this.tag.values.splice(index, 1);

        this.settings.onTagRemove(e, text);

        this._focusInput();
    };

    /**
     * Format the text for a tag
     * @param {String} text Tag text
     * @return {String}
     */
    Taggle.prototype._formatTag = function(text) {
        return this.settings.preserveCase ? text : text.toLowerCase();
    };

    Taggle.prototype.getTags = function() {
        return {
            elements: this.getTagElements(),
            values: this.getTagValues()
        };
    };

    // @todo
    // @deprecated use getTags().elements
    Taggle.prototype.getTagElements = function() {
        return this.tag.elements;
    };

    // @todo
    // @deprecated use getTags().values
    Taggle.prototype.getTagValues = function() {
        return [].slice.apply(this.tag.values);
    };

    Taggle.prototype.getInput = function() {
        return this.input;
    };

    Taggle.prototype.getContainer = function() {
        return this.container;
    };

    Taggle.prototype.add = function(text) {
        var isArr = _isArray(text);

        if (isArr) {
            for (var i = 0, len = text.length; i < len; i++) {
                if (typeof text[i] === 'string') {
                    this._add(null, text[i]);
                }
            }
        }
        else {
            this._add(null, text);
        }

        return this;
    };

    Taggle.prototype.remove = function(text, all) {
        var len = this.tag.values.length - 1;
        var found = false;

        while (len > -1) {
            var tagText = this.tag.values[len];
            if (this.settings.attachTagId) {
                tagText = tagText.text;
            }

            if (tagText === text) {
                found = true;
                this._remove(this.tag.elements[len]);
            }

            if (found && !all) {
                break;
            }

            len--;
        }

        return this;
    };

    Taggle.prototype.removeAll = function() {
        for (var i = this.tag.values.length - 1; i >= 0; i--) {
            this._remove(this.tag.elements[i]);
        }

        return this;
    };

    /* global define, module */
    if (typeof define === 'function' && define.amd) {
        // AMD
        define([], function() {
            return Taggle;
        });
    }
    else if (typeof exports === 'object') {
        // CommonJS
        module.exports = Taggle;
    }
    else {
        // Vanilla browser global
        window.Taggle = Taggle;
    }

}(window, document));

// trigger a custom event on an object
function fireEvent(obj, evt){
  var fireOnThis = obj;
  if( document.createEvent ) {
    var evObj = document.createEvent('MouseEvents');
    evObj.initEvent( evt, true, false );
    fireOnThis.dispatchEvent( evObj );
  }
  else if( document.createEventObject ) { //IE
    var evObj = document.createEventObject();
    fireOnThis.fireEvent( 'on' + evt, evObj );
  }
}


// falback for foreach
if (!Array.prototype.forEach) {

  Array.prototype.forEach = function(callback, thisArg) {

    var T, k;

    if (this == null) {
      throw new TypeError(' this is null or not defined');
    }

    // 1. Let O be the result of calling ToObject passing the |this| value as the argument.
    var O = Object(this);

    // 2. Let lenValue be the result of calling the Get internal method of O with the argument "length".
    // 3. Let len be ToUint32(lenValue).
    var len = O.length >>> 0;

    // 4. If IsCallable(callback) is false, throw a TypeError exception.
    // See: http://es5.github.com/#x9.11
    if (typeof callback !== "function") {
      throw new TypeError(callback + ' is not a function');
    }

    // 5. If thisArg was supplied, let T be thisArg; else let T be undefined.
    if (arguments.length > 1) {
      T = thisArg;
    }

    // 6. Let k be 0
    k = 0;

    // 7. Repeat, while k < len
    while (k < len) {

      var kValue;

      // a. Let Pk be ToString(k).
      //   This is implicit for LHS operands of the in operator
      // b. Let kPresent be the result of calling the HasProperty internal method of O with argument Pk.
      //   This step can be combined with c
      // c. If kPresent is true, then
      if (k in O) {

        // i. Let kValue be the result of calling the Get internal method of O with argument Pk.
        kValue = O[k];

        // ii. Call the Call internal method of callback with T as the this value and
        // argument list containing kValue, k, and O.
        callback.call(T, kValue, k, O);
      }
      // d. Increase k by 1.
      k++;
    }
    // 8. return undefined
  };
}

// Insert after function
function insertAfter(newElement,targetElement) {
  //target is what you want it to go after. Look for this elements parent.
  var parent = targetElement.parentNode;

  //if the parents lastchild is the targetElement...
  if(parent.lastchild == targetElement) {
    //add the newElement after the target element.
    parent.appendChild(newElement);
  } else {
    // else the target has siblings, insert the new element between the target and it's next sibling.
    parent.insertBefore(newElement, targetElement.nextSibling);
  }
}


// Remove element
function removeElement(targetElement) {
  //target is what you want to delete. Look for this elements parent.
  var parent = targetElement.parentNode;

  parent.removeChild(targetElement);
}

// matches polyfill
this.Element && function(ElementPrototype) {
  ElementPrototype.matches = ElementPrototype.matches ||
  ElementPrototype.matchesSelector ||
  ElementPrototype.webkitMatchesSelector ||
  ElementPrototype.msMatchesSelector ||
  function(selector) {
    var node = this, nodes = (node.parentNode || node.document).querySelectorAll(selector), i = -1;
    while (nodes[++i] && nodes[i] != node);
    return !!nodes[i];
  }
}(Element.prototype);

// closest polyfill
this.Element && function(ElementPrototype) {
  ElementPrototype.closest = ElementPrototype.closest ||
  function(selector) {
    var el = this;
    while (el.matches && !el.matches(selector)) el = el.parentNode;
    return el.matches ? el : null;
  }
}(Element.prototype);

// toggle class polyfill
function toggleClass(element, className){
  if (!element || !className){
    return;
  }

  var classString = element.className, nameIndex = classString.indexOf(className);
  if (nameIndex == -1) {
    classString += ' ' + className;
  }
  else {
    classString = classString.substr(0, nameIndex) + classString.substr(nameIndex+className.length);
  }
  element.className = classString;
}

// isNumeric
function isNumeric(n) {
  return !isNaN(parseFloat(n)) && isFinite(n);
}


// Polyfill for wrap function
function wrap(element, wrapper) {
  // Cache the current parent and sibling.
  var parent  = element.parentNode;
  var sibling = element.nextSibling;
  // Wrap the element (is automatically removed from its current
  // parent).
  wrapper.appendChild(element);
  // If the element had a sibling, insert the wrapper before
  // the sibling to maintain the HTML structure; otherwise, just
  // append it to the parent.
  if (sibling) {
    parent.insertBefore(wrapper, sibling);
  } else {
    parent.appendChild(wrapper);
  }
}


// addclass polyfill
function addClass(el, className) {
  if (el.classList)
    el.classList.add(className);
  else
    el.className += ' ' + className;
}

// hasClass polyfill
function hasClass(el, className) {
  if (el.classList)
    return el.classList.contains(className);
  else
    return new RegExp('(^| )' + className + '( |$)', 'gi').test(el.className);
}

// removeclass polyfill
function removeClass(el, className) {
  if (el.classList)
    el.classList.remove(className);
  else
    el.className = el.className.replace(new RegExp('(^|\\b)' + className.split(' ').join('|') + '(\\b|$)', 'gi'), ' ');
}

// Strip tags from element
function stripTags(html) {
  var tmp = document.createElement("DIV");
  tmp.innerHTML = html;
  return tmp.textContent || tmp.innerText;
}

// Request animation frame polyfill
(function() {
  var lastTime = 0;
  var vendors = ['webkit', 'moz'];
  for(var x = 0; x < vendors.length && !window.requestAnimationFrame; ++x) {
    window.requestAnimationFrame = window[vendors[x]+'RequestAnimationFrame'];
    window.cancelAnimationFrame =
      window[vendors[x]+'CancelAnimationFrame'] || window[vendors[x]+'CancelRequestAnimationFrame'];
  }

  if (!window.requestAnimationFrame)
    window.requestAnimationFrame = function(callback, element) {
      var currTime = new Date().getTime();
      var timeToCall = Math.max(0, 16 - (currTime - lastTime));
      var id = window.setTimeout(function() { callback(currTime + timeToCall); },
        timeToCall);
      lastTime = currTime + timeToCall;
      return id;
    };

  if (!window.cancelAnimationFrame)
    window.cancelAnimationFrame = function(id) {
      clearTimeout(id);
    };
}());

// create a unique ID
function uniqueId () {
  // desired length of Id
  var idStrLen = 32;
  // always start with a letter -- base 36 makes for a nice shortcut
  var idStr = (Math.floor((Math.random() * 25)) + 10).toString(36) + "_";
  // add a timestamp in milliseconds (base 36 again) as the base
  idStr += (new Date()).getTime().toString(36) + "_";
  // similar to above, complete the Id using random, alphanumeric characters
  do {
    idStr += (Math.floor((Math.random() * 35))).toString(36);
  } while (idStr.length < idStrLen);

  return (idStr);
}


// Array.prototype.map polyfill
// code from https://developer.mozilla.org/en-US/docs/JavaScript/Reference/Global_Objects/Array/map
if (!Array.prototype.map){
  Array.prototype.map = function(fun /*, thisArg */){
    "use strict";

    if (this === void 0 || this === null)
      throw new TypeError();

    var t = Object(this);
    var len = t.length >>> 0;
    if (typeof fun !== "function")
      throw new TypeError();

    var res = new Array(len);
    var thisArg = arguments.length >= 2 ? arguments[1] : void 0;
    for (var i = 0; i < len; i++)
    {
      // NOTE: Absolute correctness would demand Object.defineProperty
      //       be used.  But this method is fairly new, and failure is
      //       possible only if Object.prototype or Array.prototype
      //       has a property |i| (very unlikely), so use a less-correct
      //       but more portable alternative.
      if (i in t)
        res[i] = fun.call(thisArg, t[i], i, t);
    }

    return res;
  };
}


// scroll element to specific position
function scrollTo(element, to, duration, callback) {
  if (duration < 0) return;
  var difference = to - element.scrollTop;
  var perTick = difference / duration * 10;

  setTimeout(function() {
    if (element.scrollTop === to || duration <= 0){
      callback();
    }else {
      element.scrollTop = element.scrollTop + perTick;
      scrollTo(element, to, duration - 10, callback);
    }
  }, 10);
}

// This function gets a random number between min and max
function randomIntFromInterval(min,max){
  return Math.floor( Math.random() * (max - min + 1) + min);
};

// trunc polyfill
Math.trunc = Math.trunc || function(x) {
  return x < 0 ? Math.ceil(x) : Math.floor(x);
};

/**
 * Get all DOM element up the tree that contain a class, ID, or data attribute
 * @param  {Node} elem The base element
 * @param  {String} selector The class, id, data attribute, or tag to look for
 * @return {Array} Null if no match
 */
var getParents = function (elem, selector) {

  var parents = [];
  var firstChar;
  if ( selector ) {
    firstChar = selector.charAt(0);
  }

  // Get matches
  for ( ; elem && elem !== document; elem = elem.parentNode ) {
    if ( selector ) {

      // If selector is a class
      if ( firstChar === '.' ) {
        if ( elem.classList.contains( selector.substr(1) ) ) {
          parents.push( elem );
        }
      }

      // If selector is an ID
      if ( firstChar === '#' ) {
        if ( elem.id === selector.substr(1) ) {
          parents.push( elem );
        }
      }

      // If selector is a data attribute
      if ( firstChar === '[' ) {
        if ( elem.hasAttribute( selector.substr(1, selector.length - 1) ) ) {
          parents.push( elem );
        }
      }

      // If selector is a tag
      if ( elem.tagName.toLowerCase() === selector ) {
        parents.push( elem );
      }

    } else {
      parents.push( elem );
    }

  }

  // Return parents if any exist
  if ( parents.length === 0 ) {
    return null;
  } else {
    return parents;
  }

};

var getParentsUntil = function (elem, parent, selector) {

  var parents = [];
  if ( parent ) {
    var parentType = parent.charAt(0);
  }
  if ( selector ) {
    var selectorType = selector.charAt(0);
  }

  // Get matches
  for ( ; elem && elem !== document; elem = elem.parentNode ) {

    // Check if parent has been reached
    if ( parent ) {

      // If parent is a class
      if ( parentType === '.' ) {
        if ( elem.classList.contains( parent.substr(1) ) ) {
          break;
        }
      }

      // If parent is an ID
      if ( parentType === '#' ) {
        if ( elem.id === parent.substr(1) ) {
          break;
        }
      }

      // If parent is a data attribute
      if ( parentType === '[' ) {
        if ( elem.hasAttribute( parent.substr(1, parent.length - 1) ) ) {
          break;
        }
      }

      // If parent is a tag
      if ( elem.tagName.toLowerCase() === parent ) {
        break;
      }

    }

    if ( selector ) {

      // If selector is a class
      if ( selectorType === '.' ) {
        if ( elem.classList.contains( selector.substr(1) ) ) {
          parents.push( elem );
        }
      }

      // If selector is an ID
      if ( selectorType === '#' ) {
        if ( elem.id === selector.substr(1) ) {
          parents.push( elem );
        }
      }

      // If selector is a data attribute
      if ( selectorType === '[' ) {
        if ( elem.hasAttribute( selector.substr(1, selector.length - 1) ) ) {
          parents.push( elem );
        }
      }

      // If selector is a tag
      if ( elem.tagName.toLowerCase() === selector ) {
        parents.push( elem );
      }

    } else {
      parents.push( elem );
    }

  }

  // Return parents if any exist
  if ( parents.length === 0 ) {
    return null;
  } else {
    return parents;
  }

};


/**
 * TextOverflowClamp.js
 *
 * Updated 2014-05-08 to improve speed and fix some bugs.
 *
 * Updated 2013-05-09 to remove jQuery dependancy.
 * But be careful with webfonts!
 *
 * NEW!
 * - Support for padding.
 * - Support for nearby floated elements.
 * - Support for text-indent.
 */

// bind function support for older browsers without it
// https://developer.mozilla.org/en-US/docs/JavaScript/Reference/Global_Objects/Function/bind
if (!Function.prototype.bind) {
  Function.prototype.bind = function (oThis) {
    if (typeof this !== "function") {
      // closest thing possible to the ECMAScript 5 internal IsCallable function
      throw new TypeError("Function.prototype.bind - what is trying to be bound is not callable");
    }

    var aArgs = Array.prototype.slice.call(arguments, 1),
      fToBind = this,
      fNOP = function () {},
      fBound = function () {
        return fToBind.apply(this instanceof fNOP && oThis
            ? this
            : oThis,
          aArgs.concat(Array.prototype.slice.call(arguments)));
      };

    fNOP.prototype = this.prototype;
    fBound.prototype = new fNOP();

    return fBound;
  };
}

// the actual meat is here
(function(w, d){
  var clamp, measure, text, lineWidth,
    lineStart, lineCount, wordStart,
    line, lineText, wasNewLine,
    ce = d.createElement.bind(d),
    ctn = d.createTextNode.bind(d),
    width, widthChild, newWidthChild;

  // measurement element is made a child of the clamped element to get it's style
  measure = ce('span');

  (function(s){
    s.position = 'absolute'; // prevent page reflow
    s.whiteSpace = 'pre'; // cross-browser width results
    s.visibility = 'hidden'; // prevent drawing
  })(measure.style);

  // width element calculates the width of each line
  width = ce('span');

  widthChild = ce('span');
  widthChild.style.display = 'block';
  widthChild.style.overflow = 'hidden';
  widthChild.appendChild(ctn("\u2060"));

  clamp = function (el, lineClamp) {
    var i;
    // make sure the element belongs to the document
    if(!el.ownerDocument || !el.ownerDocument === d) return;
    // reset to safe starting values
    lineStart = wordStart = 0;
    lineCount = 1;
    wasNewLine = false;
    //lineWidth = el.clientWidth;
    lineWidth = [];
    // get all the text, remove any line changes
    text = (el.textContent || el.innerText).replace(/\n/g, ' ');
    // create a child block element that accounts for floats
    for(i = 1; i < lineClamp; i++) {
      newWidthChild = widthChild.cloneNode(true);
      width.appendChild(newWidthChild);
      if(i === 1) {
        widthChild.style.textIndent = 0;
      }
    }
    widthChild.style.textIndent = '';
    // cleanup
    newWidthChild = void 0;
    // remove clamped class
    removeClass(el, 'js-clamped');
    // remove all content
    while(el.firstChild)
      el.removeChild(el.firstChild);
    // ready for width calculating magic
    el.appendChild(width);
    // then start calculating widths of each line
    for(i = 0; i < lineClamp - 1; i++) {
      lineWidth.push(width.childNodes[i].clientWidth);
    }
    // we are done, no need for this anymore
    el.removeChild(width);
    // cleanup the lines
    while(width.firstChild)
      width.removeChild(width.firstChild);
    // add measurement element within so it inherits styles
    el.appendChild(measure);
    // http://ejohn.org/blog/search-and-dont-replace/
    text.replace(/ /g, function(m, pos) {
      // ignore any further processing if we have total lines
      if(lineCount === lineClamp) return;
      // create a text node and place it in the measurement element
      measure.appendChild(ctn(text.substr(lineStart, pos - lineStart)));
      // have we exceeded allowed line width?
      if(lineWidth[lineCount - 1] <= measure.clientWidth) {
        if(wasNewLine) {
          // we have a long word so it gets a line of it's own
          lineText = text.substr(lineStart, pos + 1 - lineStart);
          // next line start position
          lineStart = pos + 1;
        } else {
          // grab the text until this word
          lineText = text.substr(lineStart, wordStart - lineStart);
          // next line start position
          lineStart = wordStart;
        }
        // create a line element
        line = ce('span');
        // add text to the line element
        line.appendChild(ctn(lineText));
        // add the line element to the container
        el.appendChild(line);
        // yes, we created a new line
        wasNewLine = true;
        lineCount++;
      } else {
        // did not create a new line
        wasNewLine = false;
      }
      // remember last word start position
      wordStart = pos + 1;
      // clear measurement element
      measure.removeChild(measure.firstChild);
    });
    // remove the measurement element from the container
    el.removeChild(measure);
    // create the last line element
    line = ce('span');
    // see if we need to add styles
    if(lineCount === lineClamp) {
      // give styles required for text-overflow to kick in
      (function(s) {
        s.textIndent = 0;
        s.whiteSpace = 'nowrap';
        s.position = 'absolute';
      })(line.style);
    }
    // add all remaining text to the line element
    line.appendChild(ctn(text.substr(lineStart)));
    // add the line element to the container
    el.appendChild(line);
    if(lineCount === lineClamp) {
      // create reference
      var ref = ce('div');
      el.appendChild(ref);

      if(line.clientWidth > ref.clientWidth){
        addClass(el, 'js-clamped');
      }
      el.removeChild(ref);
      // give styles required for text-overflow to kick in
      (function(s) {
        s.display = 'block';
        s.overflow = 'hidden';
        s.textOverflow = 'ellipsis';
        s.position = 'static';
      })(line.style);
      addClass(line, 'js-clamp-last-span');
    }
  }
  w.clamp = clamp;
})(window, window.document);

(function(factory){

  window.Popover = factory();

})(function(){

  // Popover
  var Popover = function( element,options ) {
    options = options || {};
    this.isIE = (new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})").exec(navigator.userAgent) != null) ? parseFloat( RegExp.$1 ) : false;
    this.link = typeof element === 'object' ? element : document.querySelector(element);
    this.title = this.link.getAttribute('data-title') || null;
    this.content = this.link.getAttribute('data-content') || null;
    this.popover = null;
    this.options = {};
    this.options.trigger = options.trigger ? options.trigger : 'hover';
    this.options.placement = options.placement ? options.placement : 'top';
    this.options.dismiss = options.dismiss && options.dismiss === 'true' ? true : false;
    //Custom
    this.options.show = options.show ? options.show : false;

    this.duration = 150;
    this.options.duration = (this.isIE && this.isIE < 10) ? 0 : (options.duration || this.duration);
    this.options.container = document.body;
    if ( this.content ) this.init();
    this.timer = 0 // the link own event timer
    this.rect = null;
  }

  // Methods
  Popover.prototype = {

    init : function() {

      this.actions();
      if (this.options.trigger === 'hover') {
        this.link.addEventListener('mouseenter', this.open, false);
        if (!this.options.dismiss) { this.link.addEventListener('mouseleave', this.close, false); }
      } else if (this.options.trigger === 'click') {
        this.link.addEventListener('click', this.toggle, false);
        if (!this.options.dismiss) { this.link.addEventListener('blur', this.close, false); }
      } else if (this.options.trigger === 'focus') {
        this.link.addEventListener('focus', this.toggle, false);
        if (!this.options.dismiss) { this.link.addEventListener('blur', this.close, false); }
      }

      // If data-show = true show the the toggle directly
      if(this.options.show) { this.open(); };

      // Custom extras
      // By default show on focus
      this.link.addEventListener('focus', this.toggle, false);
      // When out of focus close popup again
      this.link.addEventListener('blur', this.close, false);

      if (this.options.dismiss) { document.addEventListener('click', this.dismiss, false); }

      if (!(this.isIE && this.isIE < 9) && (this.options.trigger === 'focus' || this.options.trigger === 'click') ) {
        window.addEventListener('resize', this.close, false ); } // dismiss on window resize
    },

    actions : function() {
      var self = this;

      this.toggle = function(e) {
        if (self.popover === null) {
          self.open()
        } else {
          self.close()
        }
      },
      this.open = function(e) {
        clearTimeout(self.link.getAttribute('data-timer'));
        self.timer = setTimeout( function() {
          if (self.popover === null) {
            self.createPopover();
            self.stylePopover();
            self.updatePopover()
          }
        }, self.options.duration );
        self.link.setAttribute('data-timer',self.timer);
      },
      this.dismiss = function(e) {
        if (self.popover && e.target === self.popover.querySelector('.close')) {
          self.close();
        }
      },
      this.close = function(e) {
        clearTimeout(self.link.getAttribute('data-timer'));
        self.timer = setTimeout( function() {
          if (self.popover && self.popover !== null && /in/.test(self.popover.className)) {
            self.popover.className = self.popover.className.replace(' in','');
            setTimeout(function() {
              self.removePopover(); // for performance/testing reasons we can keep the popovers if we want
            }, self.options.duration);
          }

        }, self.options.delay + self.options.duration);
        self.link.setAttribute('data-timer',self.timer);
      },

      // remove the popover
      this.removePopover = function() {
        this.popover && this.options.container.removeChild(this.popover);
        this.popover = null;
        this.timer = null
      },

      this.createPopover = function() {
        this.popover = document.createElement('div');

        if ( this.content !== null ) { //create the popover from data attributes

          this.popover.setAttribute('role','tooltip');

          var popoverArrow = document.createElement('div');
          popoverArrow.setAttribute('class','popover__arrow');

          if (this.title !== null) {
            var popoverTitle = document.createElement('h3');
            popoverTitle.setAttribute('class','popover__title');

            if (this.options.dismiss) {
              popoverTitle.innerHTML = this.title + '<button type="button" class="popover__close">×</button>';
            } else {
              popoverTitle.innerHTML = this.title;
            }
            this.popover.appendChild(popoverTitle);
          }

          var popoverContent = document.createElement('div');
          popoverContent.setAttribute('class','popover__content');

          this.popover.appendChild(popoverArrow);
          this.popover.appendChild(popoverContent);

          //set popover content
          if (this.options.dismiss && this.title === null) {
            popoverContent.innerHTML = this.content + '<button type="button" class="popover__close">×</button>';
          } else {
            popoverContent.innerHTML = this.content;
          }
        }

        //append to the container
        this.options.container.appendChild(this.popover);
        this.popover.style.display = 'block';



      },

      this.stylePopover = function(pos) {
        this.rect = this.getRect();
        var placement = pos || this.options.placement;
        var animation = 'fade';
        var size = '';

        //Create extra "size" item
        if(this.content.length > 80){
          size = 'popover--large';
        }
        this.popover.setAttribute('class','popover popover--'+placement+' '+animation+ ' '+size);

        var linkDim = { w: this.link.offsetWidth, h: this.link.offsetHeight }; //link real dimensions

        // all popover dimensions
        var pd = this.popoverDimensions(this.popover);
        var toolDim = { w : pd.w, h: pd.h }; //popover real dimensions


        //window vertical and horizontal scroll

        var scrollYOffset = this.getScroll().y;
        var scrollXOffset =  this.getScroll().x;

        //apply styling
        if ( /top/.test(placement) ) { //TOP
          this.popover.style.top = this.rect.top + scrollYOffset - toolDim.h + 'px';
          this.popover.style.left = this.rect.left + scrollXOffset - toolDim.w/2 + linkDim.w/2 + 'px'

        } else if ( /bottom/.test(placement) ) { //BOTTOM
          this.popover.style.top = this.rect.top + scrollYOffset + linkDim.h + 'px';
          this.popover.style.left = this.rect.left + scrollXOffset - toolDim.w/2 + linkDim.w/2 + 'px';

        } else if ( /left/.test(placement) ) { //LEFT
          this.popover.style.top = this.rect.top + scrollYOffset - toolDim.h/2 + linkDim.h/2 + 'px';
          this.popover.style.left = this.rect.left + scrollXOffset - toolDim.w + 'px';

        } else if ( /right/.test(placement) ) { //RIGHT
          this.popover.style.top = this.rect.top + scrollYOffset - toolDim.h/2 + linkDim.h/2 + 'px';
          this.popover.style.left = this.rect.left + scrollXOffset + linkDim.w + 'px';
        }
      },

      this.updatePopover = function() {
        var placement = null;
        if ( !self.isElementInViewport(self.popover) ) {
          placement = self.updatePlacement();
        } else {
          placement = self.options.placement;
        }

        self.stylePopover(placement);

        self.popover.className += ' in';
      },
      this.updatePlacement = function() {
        var pos = this.options.placement;
        if ( /top/.test(pos) ) { //TOP
          return 'bottom';
        } else if ( /bottom/.test(pos) ) { //BOTTOM
          return 'top';
        } else if ( /left/.test(pos) ) { //LEFT
          return 'right';
        } else if ( /right/.test(pos) ) { //RIGHT
          return 'left';
        }
      },
      this.getRect = function() {
        return this.link.getBoundingClientRect()
      },
      this.getScroll = function() {
        return {
          y : window.pageYOffset || document.documentElement.scrollTop,
          x : window.pageXOffset || document.documentElement.scrollLeft
        }
      },
      this.popoverDimensions  = function(p) {//check popover width and height
        return {
          w : p.offsetWidth,
          h : p.offsetHeight
        }
      },
      this.isElementInViewport = function(t) { // check if this.popover is in viewport
        var r = t.getBoundingClientRect();
        return (
          r.top >= 0 &&
          r.left >= 0 &&
          r.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
          r.right <= (window.innerWidth || document.documentElement.clientWidth)
        )
      }
    }
    }

  // POPOVER DATA API
  // =================
    var Popovers = document.querySelectorAll('[data-toggle=popover]'), i = 0, ppl = Popovers.length;

  for (i;i<ppl;i++){
    var item = Popovers[i], options = {};
    options.trigger = item.getAttribute('data-trigger'); // click / hover / focus
    options.duration = item.getAttribute('data-duration');
    options.placement = item.getAttribute('data-placement');
    options.dismiss = item.getAttribute('data-dismiss');
    options.show = item.getAttribute('data-show');
    new Popover(item,options);
  }

  return Popover;
});


var vl = {};


(function() {
  "use strict";
  /**
   * Add js class to the HTML
   */
  addClass(document.querySelectorAll('html')[0], 'js');
  removeClass(document.querySelectorAll('html')[0], 'no-js');

  /**
   * Add eventlisteners to window
   */
  window.addEventListener('resize', windowResize);

  // transfer css breakpoints to JS
  vl.breakpoint = {};
  vl.breakpoint.refreshValue = function () {
    this.value = window.getComputedStyle(document.querySelector('body'), ':before').getPropertyValue('content').replace(/\"/g, '');
  };

  /**
   * Call the initial functions when page is loaded
   */
  window.addEventListener('load', function() {
    // Call infographic text sizing
    infographicFitText();
    //Shows content when checkbox is checked
    showContentOnChange();
    // highlight double input labels
    highlightDoubleInputs();

  });

  /**
   * all functions to do on window resize
   */
  function windowResize() {
    infographicFitText();
    vl.breakpoint.refreshValue();
  }

  /**
   * Show content when a linked radio / checkbox is checkednpm
   */
  function showContentOnChange(){
    var elements = document.querySelectorAll('[data-show-checked-target]');

    [].forEach.call(elements, function(element) {
      // init without event
      showChecked(element);

      // if the target is a radio-button, trigger the change event on the other radio-buttons in the group
      if(element.type === "radio"){
        var relatedRadios = document.getElementsByName(element.name);
        [].forEach.call(relatedRadios, function(radio) {

          if(!radio.hasAttribute('data-show-bound')) {
            radio.addEventListener('change', function (e) {
              var relatedRadios = document.getElementsByName(e.target.name);
              [].forEach.call(relatedRadios, function (radio) {
                if(radio.hasAttribute('data-show-checked-target')) {
                  showChecked(radio);
                }
              });
            });
            radio.setAttribute('data-show-bound', true);
          }
        });
      }else {
        element.addEventListener('change', function (e) {
          e.preventDefault();
          showChecked(e.target);
        }, false);
      }

      function showChecked (el){
        var target = document.querySelectorAll('[data-show-checked-trigger="' + el.getAttribute('data-show-checked-target') + '"]')[0];

        if(el.checked){
            addClass(target, 'js-show-checked--open');
        }else{
            removeClass(target, 'js-show-checked--open');
        }
      }

    });
  }

  /**
   * Double input, highlight the active top-label
   */

  // create global function
  vl.dressDoubleInput = function(element) {
    var activeClass = 'js-double-input-top-label--active';
    // add focus event to the input fields
    var inputs = element.querySelectorAll('[data-double-input-label-highlight]');
    // add events
    [].forEach.call(inputs, function(input) {
      input.addEventListener('focus', function() {
        var labelID = input.getAttribute('data-double-input-label-highlight');
        var label = element.querySelector('[data-double-input-label="'+labelID+'"]');
        addClass(label, activeClass);
      });
      input.addEventListener('blur', function() {
        var labelID = input.getAttribute('data-double-input-label-highlight');
        var label = element.querySelector('[data-double-input-label="'+labelID+'"]');
        removeClass(label, activeClass);
      });
    });
  };

  function highlightDoubleInputs() {
    // get all elements with a data-attribute data-clamp
    var elements = document.querySelectorAll('.js-double-input');
    // apply highlighting for all fields
    [].forEach.call(elements, function(element) {
      vl.dressDoubleInput(element);
    });
  }


  /**
   * Change the font-size of the value of an infographic to fit the container
   */
  function infographicFitText(){
    // get all infographic elements
    var elements = document.querySelectorAll('.js-infographic__value');
    // set the text-size for the value of each infographic
    [].forEach.call(elements, function(element) {
      // Add dots in large numbers.
      var number = element.innerHTML;
      number = number.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1.");
      element.innerHTML = number;
      textFit(element, {maxFontSize: 50});
    });
  }
})();

vl.accordion = {};

/*
 * vl.accordion.dress(element)
 * Initiates an accordion
 * @param 1: ".js-accordion__toggle" DOM element
 */
vl.accordion.dress = function(element) {
    element.addEventListener('click', function(e) {
      e.preventDefault();
      // get the closest accordion parent
      var accordion = this.closest('.js-accordion');
      toggleClass(accordion, 'js-accordion--open');
    });
};

/*
 * vl.accordion.dressAll()
 * Initiates all accordions in the DOM
 */
vl.accordion.dressAll = function() {
  // get all accordion toggle elements
  var elements = document.querySelectorAll('.js-accordion__toggle');
  // add functionality to the accordions
  [].forEach.call(elements, function(element) {
    vl.accordion.dress(element);
  });
};

// Initiate on load
(function(){
  vl.accordion.dressAll();
})();

vl.popup = {};

/*
 * vl.popup.dress(element, id)
 * Initiates a popup
 * @param 1: ".js-accordion__toggle" DOM element
 * @param 2: "id" unique ID
 */
vl.popup.dress = function(element, id){
  // add aria tags
  element.setAttribute('aria-expanded', false);
  element.setAttribute('aria-controls', "popup-"+id);
  // get the closest popup parent
  var popup = element.closest('.js-popup');
  // add accessibility
  popup.setAttribute('id', "popup-"+id);

  element.addEventListener('click', function(e) {
    e.preventDefault();
    if(!hasClass(popup, 'js-popup--open')) {
      // close other popups
      fireEvent(document, 'click');
      addClass(popup, 'js-popup--open');
      element.setAttribute('aria-expanded', true);
    }else {
      // toggle class
      removeClass(popup, 'js-popup--open');
      element.setAttribute('aria-expanded', false);
    }
  });

  popup.addEventListener('click', function(e){
    e.stopPropagation();
  });

  document.addEventListener('click', function(){
    removeClass(popup, 'js-popup--open');
    element.setAttribute('aria-expanded', false);
  });

  document.addEventListener('keyup', function(event) {
    if (event.ctrlKey && event.which === 72) {
      removeClass(popup, 'js-popup--open');
      element.setAttribute('aria-expanded', false);
    }
  });
};

/*
 * vl.popup.dressAll()
 * Initiates all popups in the DOM
 */
vl.popup.dressAll = function(){
    var elements = document.querySelectorAll('.js-popup__toggle');
    var id = 0;

    [].forEach.call(elements, function(element) {
      vl.popup.dress(element, id);
      id++;
    });
};

// Initiate on load
(function(){
  vl.popup.dressAll();
})();

(function() {
  // initialize on pageLoad
  window.addEventListener('load', function() {
    // Call text clamping
    textOverflowClamp();
    // bind on resize event
    window.addEventListener('resize', textOverflowClamp);
  });

  var clampClass = 'js-clamped',
      noTriggerClass = 'js-clamp-useless';

  /**
   * Clamp text to the given number of lines
   */
  function textOverflowClamp(){
    // get all elements with a data-attribute data-clamp
    var elements = document.querySelectorAll('[data-clamp]');
    // apply clamping for all attributes
    [].forEach.call(elements, function(element) {
      var clampSize = parseInt(element.getAttribute("data-clamp"));
      if(clampSize <= 0){
        return;
      }
      // do the actual clamping
      clamp(element, clampSize);
      // check if there is actually content clamped, we need this to decide if the read-more button is usefull or not.
      if(element.hasAttribute('data-clamp-id')){
        // get the trigger
        var trigger = document.querySelector('[data-clamp-target="' + element.getAttribute('data-clamp-id') + '"]');
        // check if the trigger exists and has not already been bound
        if(trigger !== null && !hasClass(trigger, 'js-clamp-bound')){
          addClass(trigger, 'js-clamp-bound');
          // attach the event listener
          trigger.addEventListener('click', function(e){
            e.preventDefault();
            undoOverflowClamp(element, this);
          });
        }
        // show / hide the button if it is required
        if(hasClass(element, clampClass)){
          // the item is clamped so it can be expanded
          removeClass(trigger, noTriggerClass);
        }else{
          addClass(trigger, noTriggerClass);
        }
      }
    });
  }


  /**
   * Dress all "read more" buttons to be able to undo clamping
   */
  function undoOverflowClamp(element, trigger){
    // set raw text in the element
    var text = (element.textContent || element.innerText);
    element.textContent = text;
    // remove data-clamp attribute
    element.removeAttribute('data-clamp');
    element.removeAttribute('data-clamp-id');
    // remove clamped class
    removeClass(element, clampClass);
    // delete the button, it has become completely useless
    trigger.parentNode.removeChild(trigger);
  }
})();

/**
 * Create a tag input by data-attribute
 */

vl.dressHeroNav;

(function () {

  var navigations = document.querySelectorAll('.js-hero-navigation');


  vl.dressHeroNav = function(nav) {

    var elements = nav.querySelectorAll('.js-hero-navigation-listitem');
    var links = nav.querySelectorAll('.js-hero-navigation-link');
    var backgrounds = nav.querySelectorAll('.js-hero-navigation-background');
    var defaultClass = 'js-hero-navigation-default-link';
    var activeslide;

    function changeSlide(){
      //Change background of slide
      [].forEach.call(backgrounds, function(bg) {
        removeClass(bg, 'hero-navigation__background--active');
      });
      var activebg = document.querySelectorAll('[data-hero-index="' + activeslide + '"]');
      addClass(activebg[0], 'hero-navigation__background--active');
    }

    [].forEach.call(links, function (element) {
      dressHeroNavLink(element);
    });

    // trigger default element
    var defaultLink = nav.getElementsByClassName(defaultClass)[0];
    if(defaultLink) {
      fireEvent(defaultLink, 'mouseenter');
    } else {
      // if no default given, select a random link
      var rand = Math.floor(Math.random() * links.length);
      fireEvent(links[rand], 'mouseenter');
    }

    function dressHeroNavLink(element) {

      element.addEventListener('mouseenter', activateSlide);
      element.addEventListener('focus', activateSlide);

      function activateSlide(event) {
        var listitem = event.target.parentNode;

        [].forEach.call(elements, function (el2) {
          removeClass(el2, 'hero-navigation__list__listitem--active');
        });

        var index = listitem.getAttribute("data-hero-index");
        activeslide = index;
        addClass(listitem, 'hero-navigation__list__listitem--active');

        changeSlide();
      }
    }
  };



  [].forEach.call(navigations, function (element) {
    vl.dressHeroNav(element);
  });

})();

/**
 * Functional header menu functionality (responsive)
 */

vl.dressFHMenu;

(function () {

  var functionalheadermenus     = document.querySelectorAll('.js-functional-header-menu');
  var activeState               = 'functional-header__menu--open';

  vl.dressFHMenu = function(menu) {

    var toggle = menu.querySelector('.js-functional-header-menu__toggle');

    toggle.addEventListener('click', function(e){
      if(hasClass(menu, activeState))
        removeClass(menu, activeState);
      else
        addClass(menu, activeState);
    });
  };

  [].forEach.call(functionalheadermenus, function (element) {
    vl.dressFHMenu(element);
  });

})();

/**
 * Custom form validation extensions using the fiveForms plugin
 */


(function () {
  var forms = document.querySelectorAll('[data-validate-form]');


  // custom validators

  // Validate an IBAN number
  fiveForms.prototype.iban = function (val) {

    var regex = /\s/g,
      replacement = '',
      iban = val.toString();

    iban = iban.replace(regex, replacement);

    return !IBAN.isValid(iban);
  };

  // Validate a date
  fiveForms.prototype.date = function (val) {
    var date = val.toString(),
      pattern = /(^(((0[1-9]|1[0-9]|2[0-8])[\.](0[1-9]|1[012]))|((29|30|31)[\.](0[13578]|1[02]))|((29|30)[\.](0[4,6,9]|11)))[\.](19|[2-9][0-9])\d\d$)|(^29[\.]02[\.](19|[2-9][0-9])(00|04|08|12|16|20|24|28|32|36|40|44|48|52|56|60|64|68|72|76|80|84|88|92|96)$)/;

    return !(pattern.test(date));
  };

  // Validate a phone number
  fiveForms.prototype.phone = function (val) {
    var phone = val.toString(),
      pattern = /^((\+|00)\s{0,1}32\s?|0)(\d\s?\d{3}|\d{2}\s?\d{2})(\s?\d{2}){2}$|^((\+|00)32\s?|0)4(60|[789]\d)(\s?\d{2}){3}$/gi;

    return !(pattern.test(phone));
  };

  // Validate a RRN number (Rijksregister)
  fiveForms.prototype.rrn = function (val) {
    var rrn = val.toString(),
      pattern = /^((([0-9]{2})\.){2})([0-9]{2})[\-][0-9]{3}\.[0-9]{2}/gi;

    return !(pattern.test(rrn));
  };

  // Validate an ON number (ondernemingsnummer)
  fiveForms.prototype.onr = function (val) {
    var onr = val.toString(),
      pattern = /^0(\d{3}\.){2}\d{3}/gi;

    return !(pattern.test(onr));
  };

  // Validate a web-uri
  // todo
  fiveForms.prototype.url = function (val) {
    console.log("URL");
  };

  // Validate a number
  fiveForms.prototype.number = function (val) {
    // todo: min & max-attributes checking
    console.log("NUMBER");
  };


  // indicate an error for a field
  function showError(validatedField) {
    var vf = validatedField;
    addClass(vf.obj, vf.obj.getAttribute('data-error-class'));
    // show form error if it is defined
    if (vf.obj.hasAttribute('data-error-message') && vf.obj.hasAttribute('data-error-placeholder') && document.querySelector('[data-error-id=' + vf.obj.getAttribute('data-error-placeholder') + ']')) {
      (document.querySelector('[data-error-id=' + vf.obj.getAttribute('data-error-placeholder') + ']')).innerHTML = vf.obj.getAttribute('data-error-message');
    }
    // add error data-attribute
    vf.obj.setAttribute('data-has-error', true);
  }

  // indicate an error for a field
  function clearError(validatedField) {
    var vf = validatedField;
    removeClass(vf.obj, vf.obj.getAttribute('data-error-class'));
    if (vf.obj.hasAttribute('data-error-message') && vf.obj.hasAttribute('data-error-placeholder') && document.querySelector('[data-error-id=' + vf.obj.getAttribute('data-error-placeholder') + ']')) {
      (document.querySelector('[data-error-id=' + vf.obj.getAttribute('data-error-placeholder') + ']')).innerHTML = '';
    }
    // add error data-attribute
    vf.obj.setAttribute('data-has-error', false);
  }

  // what to do on false submit
  function formError(errors) {
    var errors; //errors is an array of the invalid form fields
    for (var x = 0; x < errors.length; x++) {
      showError(errors[x].obj);
    }
  }


  // initiate validation for the forms
  [].forEach.call(forms, function (form) {              // todo: https://toddmotto.com/ditch-the-array-foreach-call-nodelist-hack/
    // if the form does not have an ID, create one in JS
    if (!form.id) {
      form.setAttribute('id', uniqueId());
    }
    var validate = new fiveForms({
      id: form.id,   // forms always need to have an ID
      onBlur: blurred,
      onError: formError
    });
    validate.addCustomValidator('iban', validate.iban);
    validate.addCustomValidator('date', validate.date);
    validate.addCustomValidator('phone', validate.phone);
    validate.addCustomValidator('rrn', validate.rrn);
    validate.addCustomValidator('onr', validate.onr);
    validate.addCustomValidator('url', validate.url);
    validate.addCustomValidator('number', validate.number);

    function blurred(params) {
      var vf = validate.validateField(validate.getFieldByName(params.srcElement.name)); // get validated field

      if (vf.error) {
        showError(vf);
      } else {
        clearError(vf);
      }
    }

  });
})();

/**
 * Input patterns script
 */

vl.patternInput;

(function () {

  var inputs = document.querySelectorAll('[data-pattern]');

  // predefined patterns
  var patterns = {
    'iban': '{{aa}}{{99}} {{****}} {{9999}} {{999}}{{*}} {{****}} {{****}} {{****}} {{***}}',
    'rrn': '{{99}}.{{99}}.{{99}}-{{999}}.{{99}}',
    'date': '{{99}}.{{99}}.{{9999}}',
    'onr' : '{{9999}}.{{999}}.{{999}}' // ondernemingsnummer, first digit needs to be zero
  };


  // define global function
  vl.patternInput = function(input) {
    // get pattern
    var patternID = input.getAttribute('data-pattern');
    var pattern = patternID;
    if(patternID in patterns){
      pattern = patterns[patternID];
    }

    var formatted = new Formatter(input, {
      'pattern': pattern,
      'persistent': false
    });
  };


  // initiate validation for the inputs

  [].forEach.call(inputs, function(input) {
    vl.patternInput(input);
  });

})();

/**
 * Autocomplete to be used on an input field
 *
 ** Json Records have the following structure:
 ** {
 **  "title": "<mark>Dit</mark> is een programmeertaal 1",
 **  "subtitle": "Dit is een subtitel van de programmeertaal", (subtitle is optional)
 **  "value": "Programmeertaal 1",
 **  },
 **/

vl.autocomplete = {};
vl.autocomplete.dress;

(function () {

  /*
  * AC variables
  */
  var acFields    = document.querySelectorAll('[data-autocomplete]'),
      acRecord    = 'data-record';

  vl.autocomplete.dress = function(acField, customJsonFunction) {

    /** AC specific variables **/
    var arrVars = generateAutocomplete(acField),
        inputVal,
        lastVal,
        curIndex      = 0,
        ntChars       = arrVars.ntChars,
        acInput       = arrVars.acInput,
        acId          = arrVars.acId,
        acContent     = arrVars.acContent,
        acList        = arrVars.acList,
        acFocusElems  = acField.querySelectorAll('[data-focus]'),
        acRecords     = acList.querySelectorAll('['+ acRecord +']'),
        acLoader      = acField.querySelector('[data-loader]');

    /** AC specific event handlers **/
    acInput.addEventListener('keydown', acInputKeydownHandler);
    acInput.addEventListener('keyup',   acInputKeyupHandler);
    [].forEach.call(acFocusElems, function(acFocusElem){
      acFocusElem.addEventListener('blur', acFocusElemBlurHandler);
    });

    /*
    * Autocomplete input field keydown event
    */
    function acInputKeydownHandler(e){
      // Stop action on enter, arrow down & up
      switch(e.keyCode){
        case 13: case 38: case 40:
          e.preventDefault();
        break;
      }
    };

    /*
    * Autocomplete input field keyup event
    */
    function acInputKeyupHandler(e){
      inputVal = acInput.value;
      switch(e.keyCode){
        // Used to hijack the "enter" key when selecting an element in the dropdown
        case 13:
          e.preventDefault();
          setAcContentState(acField, "hide");
        break;

        // "Esc" key
        case 27:
          e.preventDefault();
          acInput.setAttribute('aria-expanded', false);
          setAcContentState(acField, "hide");
        break;

        // "arrow down" key
        case 40:
          e.preventDefault();
          moveFocus("down");
        break;

        // "arrow up" key
        case 38:
          e.preventDefault();
          moveFocus("up");
        break;

        default:
          if(this.value !== lastVal){
            // Check if value is longer than the min amount of characters
            if(this.value.length >= ntChars){
              curIndex = 0;

              // Start preloader
              setPreloader(acLoader, acField, true);

              // Retrieve data
              var jsonData = customJsonFunction(acField, acInput.value);

              // Generate records from data
              if(jsonData !== null){
                generateAutocompleteRecords(acField, jsonData);
              }

              // Stop preloader records from data
              setPreloader(acLoader, acField, false);
            }else{
              setAcContentState(acField, "hide");
            }
          }
          // Check if value is changed
          lastVal = this.value;
        break;
      }
    };

    /*
    * moveFocus()
    * @param : direction (string)
    */
    function moveFocus(direction){
      if(acInput.getAttribute('aria-expanded') == "true"){
        var countElems = acList.querySelectorAll('['+ acRecord +']').length;

        switch(direction){
          case "down":
            if(curIndex < countElems){
              curIndex++;
              setValue();
            }
          break;

          case "up":
            if(curIndex > 1){
              curIndex--;
              setValue();
            }
          break;
        }

        function setValue(){
          [].forEach.call(acList.querySelectorAll('[data-record]'), function(item){ removeClass(item, 'autocomplete__cta--focus') });
          var target = acList.querySelector('[data-record][data-index="'+ curIndex +'"]');
          addClass(target, 'autocomplete__cta--focus');
          acInput.value = target.getAttribute('data-value');
          target.focus();
          acInput.focus();
        }
      }
    }

    /*
    * acFocusElemBlurHandler();
    * Used to check the focus state and close the autocomplete when focus is outside of the element
    */
    function acFocusElemBlurHandler(e){
      window.setTimeout(function(){
        var parent = document.activeElement.closest('[data-autocomplete][data-id="' + acId + '"]');
        if(parent === null){
          setVisibilityAttributes(acContent, false, true);
          acInput.setAttribute('aria-expanded', false);
        }
      }, 1);
    };

  };

  /*
  * generateAutocompleteRecords();
  * @param acField: Autocomplete div (.js-select)
  * @param data: JSON object {*title: "", subtitle: "", *value: ""}
  */
  function generateAutocompleteRecords(acField, data) {

    if(data !== null){
      var ntChars = acField.getAttribute('[data-input]'),
          acInput       = acField.querySelector('[data-input]'),
          acContent     = acField.querySelector('[data-content]'),
          acList        = acField.querySelector('[data-records]'),
          acLoader      = acField.querySelector('[data-loader]');

      /* #1 Remove current children if existent */
      var acList = acField.querySelector('[data-records]');
      while (acList.firstChild) {
          acList.removeChild(acList.firstChild);
      }

      /* #2 Generate children based on the data */
      var c = 1;
      // if(acField.hasAttribute('data-location')){
      //   // Logica voor "jouw locatie" in te stellen hier

      //   var acRecord = document.createElement('li');
      //   addClass(acRecord, 'autocomplete__item');
      //   acRecord.setAttribute('role','option');
      //   acRecord.innerHTML = '<a class="autocomplete__cta" href="#" tabindex="-1" data-index="' + c + '" data-record data-focus data-value="Antwerpen">' +
      //                             '<span class="autocomplete__cta__title autocomplete__cta__title--location">' + acField.getAttribute('data-location-ph') + '</span>' +
      //                          '</a>';

      //   acRecord.addEventListener('click', function(e){
      //     e.preventDefault();
      //     var rec = acRecord.querySelector('[data-record]');
      //     acInput.value = rec.getAttribute('data-value');
      //     setVisibilityAttributes(acContent, false, true);
      //     acInput.setAttribute('aria-expanded', false);
      //   });

      //   acList.appendChild(acRecord);
      //   c++;
      // }

      generateRecords();

      function generateRecords(){

        [].forEach.call(data, generateRecord);
        function generateRecord(obj){
          // Generate list item
          var acRecord = document.createElement('li');
          addClass(acRecord, 'autocomplete__item');
          acRecord.setAttribute('role','option');

          // Append html to the record
          if(typeof obj.subtitle !== "undefined"){
          acRecord.innerHTML = '<a class="autocomplete__cta" href="#" tabindex="-1" data-index="' + c + '" data-record data-focus data-value="' + obj.value + '">' +
                                  '<span class="autocomplete__cta__sub">' + obj.subtitle + '</span>' +
                                  '<span class="autocomplete__cta__title">' + obj.title + '</span>' +
                               '</a>';
          }else{
            acRecord.innerHTML = '<a class="autocomplete__cta" href="#" tabindex="-1" data-index="' + c + '" data-record data-focus data-value="' + obj.value + '">' +
                                  '<span class="autocomplete__cta__title">' + obj.title + '</span>' +
                                 '</a>';
          }

          /* #2.1 Click event for child */
          acRecord.addEventListener('click', function(e){
            e.preventDefault();
            var rec = acRecord.querySelector('[data-record]');
            acInput.value = rec.getAttribute('data-value');
            setVisibilityAttributes(acContent, false, true);
            acInput.setAttribute('aria-expanded', false);
          });

          acList.appendChild(acRecord);
          c++;
        }
      }


      /* #3 Show content */
      setVisibilityAttributes(acContent, true, false);
      acInput.setAttribute('aria-expanded', "true");
    }
  };

  /*
  * setVisibilityAttributes()
  * Setting data attributes & aria tags
  * @param field        (DOM element)
  * @param dataShow     (bool)
  * @param ariaHidden   (bool)
  */
  function setVisibilityAttributes(field, dataShow, ariaHidden){
    field.setAttribute('data-show',   dataShow);
    field.setAttribute('aria-hidden', ariaHidden);
  };

  /*
  * setPreloader()
  * Showing/hiding the preloader on the autocomplete input field
  * @param acLoader   (DOM element)
  * @param acField    (DOM element)
  * @param isloading  (bool) sets/unsets preloader in autocomplete input field
  */
  function setPreloader(acLoader, acField, isLoading){
    if(isLoading){
      setVisibilityAttributes(acLoader, true, false);
    }else{
      setVisibilityAttributes(acLoader, false, true);
    }

    acField.setAttribute('data-loading', isLoading);
  };

  /*
  * setAcState
  * @param acField    (DOM element)
  * @param acState    (bool)
  */
  function setAcContentState(acField, acState){
    var acContent = acField.querySelector('[data-content]');
    switch(acState){
      case "show":
        setVisibilityAttributes(acContent, true, false);
      break;

      case "hide":
        setVisibilityAttributes(acContent, false, true);
      break;
    }
  };

  /*
  * generateAutocomplete()
  * @param acField  (DOM element)
  */
  function generateAutocomplete(acField){

    var acId = uniqueId();
    acField.setAttribute('data-id', acId);

    var arr, ntChars = detectCharacterCount(acField);
    var acInput = setAcInput(acField);
    var acLoader = setAcLoader(acField);
    var acContent = setAcContent(acField, acInput);
    var acListWrapper = setAcListWrapper(acContent);
    var acList = setAcList(acListWrapper);

      /* detect charcount */
    function detectCharacterCount(acField){
      if(acField.hasAttribute('data-min-chars') && isNumeric(acField.getAttribute('data-min-chars'))){
        return acField.getAttribute('data-min-chars');
      }else{
        return 3;
      }
    }

    /* modify the given input field */
    function setAcInput(acField){
      var acInput = acField.querySelector('input');
      (!acInput.hasAttribute('id') ? acInput.setAttribute('id', 'autocomplete-input-' + uniqueId()) : null);
      acInput.setAttribute('aria-expanded', "false");
      acInput.setAttribute('data-focus', '');
      acInput.setAttribute('data-input', '');
      acInput.setAttribute('autocapitalize', 'off');
      acInput.setAttribute('spellcheck', 'off');
      acInput.setAttribute('autocomplete', 'off');
      acInput.setAttribute('aria-autocomplete', 'list');
      acInput.setAttribute('aria-owns', 'autocomplete-' + acId);
      acInput.setAttribute('aria-controls', 'autocomplete-' + acId);
      acInput.setAttribute('aria-haspopup', 'listbox');

      return acInput;
    }

    function setAcLoader(acField){
        var acLoader = document.createElement("div");
        addClass(acLoader, 'autocomplete__loader');
        acLoader.setAttribute('data-show','false');
        acLoader.setAttribute('data-loader','');
        acLoader.setAttribute('aria-hidden',true);
        acField.appendChild(acLoader);

        return acLoader;
    }

    /* generate the accontent field */
    function setAcContent(acField, acInput){

      var acContent = document.createElement('div');
      addClass(acContent, 'autocomplete');
      acContent.setAttribute('data-content', '');
      acContent.setAttribute('aria-hidden', true);
      acContent.setAttribute('data-show', false);
      acContent.setAttribute('aria-labelledby', acInput.getAttribute('id'));
      acContent.setAttribute('id', 'autocomplete-' + acId);
      acField.appendChild(acContent);

      return acContent;
    }

    /* generate the aclistwrapper field */
    function setAcListWrapper(acContent){

      var acListWrapper = document.createElement('div');
      addClass(acListWrapper, 'autocomplete__list-wrapper');
      acContent.appendChild(acListWrapper);

      return acListWrapper;
    }

    /* generate the aclistwrapper field */
    function setAcList(acListWrapper){

      var acList = document.createElement('ul');
      addClass(acList, 'autocomplete__list');
      acList.setAttribute('data-records', '');
      acList.setAttribute('role','listbox');
      acListWrapper.appendChild(acList);

      return acList;
    }

    return {"acId": acId, "acLoader": acLoader, "ntChars": ntChars, "acInput": acInput, "acContent": acContent, "acListWrapper": acListWrapper, "acList": acList};
  };

})();


/**
 * datatable functionalities
 */

vl.datatable = {};
vl.datatable.dress;

(function () {

  var tables              = document.querySelectorAll('[data-table]'),
      dataCheckedChkboxes = "[data-table-checkbox]:checked",
      dataTableChk        = "[data-table-checkbox]";
      dataActions         = "[data-table-action]";

    vl.datatable.dress = function(table) {

      var chkboxes                  = table.querySelectorAll(dataTableChk),
          mainChkbox                = table.querySelector('[data-table-check-all]'),
          dataRowSelectable         = table.querySelectorAll("[data-table-selectable]"),
          dataRowSelectableAnchors  = table.querySelectorAll("[data-table-selectable] a, [data-table-selectable] button, [data-table-selectable] input, [data-table-selectable] label");

      // Detect change in general checkbox
      if(mainChkbox !== null){
        mainChkbox.addEventListener('change', function(e){

          if(chkboxes !== null){
            [].forEach.call(chkboxes, function(chk){
              chk.checked = mainChkbox.checked;
            });
          }
        });
      }

      // Detect change in normal checkbox
      if(chkboxes !== null){
        [].forEach.call(chkboxes, function(chk){
          chk.addEventListener('change', function(e){
            toggleActions();
            if(!e.target.checked){
              if(mainChkbox !== null)
                mainChkbox.checked = false;
            }
          });
        });
      }

      // Detect clicks on data-table-selectable attribute
      [].forEach.call(dataRowSelectable, function(row){
        row.addEventListener('click', function(e){
          var chk = row.querySelector(dataTableChk);
          if(chk !== null){
            chk.checked = !chk.checked;
            toggleActions();
          }
        });
      });

      // Detect clicks on anchors, buttons, input elements and labels and stop propagation
      [].forEach.call(dataRowSelectableAnchors, function(anchor){
        anchor.addEventListener('click', function(e){
          if(e.stopPropagation)
            e.stopPropagation();
          else
            e.cancelBubble = true;
        });
      });

      /*
      *
      * toggleActions()
      *
      */
      function toggleActions(){
        var actions = table.querySelectorAll(dataActions);
        if(actions !== null){
          var checkedChkboxes = table.querySelectorAll(dataCheckedChkboxes);
          [].forEach.call(actions, function(action){
            if(checkedChkboxes.length){
              setActionsAttributes(action, true, false);
            }else{
              setActionsAttributes(action, false, true);
            }
          });
        }
      }

      /*
      *
      * setActionsAttributes()
      * action, dataShow, ariaHidden
      */
      function setActionsAttributes(action, dataShow, ariaHidden){
        action.setAttribute('data-show', dataShow);
        action.setAttribute('aria-hidden', ariaHidden);
      }
    };

    [].forEach.call(tables, function(table) {
      vl.datatable.dress(table);
    });

})();

/**
 * Progressively enhance an input field with a JS datepicker if possible
 */

// create empty datepicker object to attach global functions
vl.datepicker = {};
vl.datepicker.dress;
vl.datepicker.getDate;

// Keep supporting the older global vl.dressdatepicker function
vl.dressDatepicker = function() {
  var args = Array.prototype.slice.call(arguments).splice(1);
  var allArguments = args.concat(Array.prototype.slice.call(arguments));
  return vl.datepicker.dress.apply(this, allArguments);
};

(function () {

  var pickerFields = document.querySelectorAll('[data-datepicker]');
  var pickerFieldClass = 'input-field--datepicker';
  var pickerIconClass = 'datepicker__icon';
  var dateFormat = 'DD.MM.YYYY';

  var i18n = {
    previousMonth : 'Vorige maand',
    nextMonth     : 'Volgende maand',
    months        : ['januari','februari','maart','april','mei','juni','juli','augustus','september','oktober','november','december'],
    weekdays      : ['zondag','maandag','dinsdag','woensdag','donderdag','vrijdag','zaterdag'],
    weekdaysShort : ['zon','maa','din','woe','don','vri','zat']
  };

  vl.datepicker.dress = function(field) {
    var picker = new Pikaday({
      field: field,
      format: dateFormat,
      i18n: i18n,
      firstDay: 1,
      minDate: moment(field.getAttribute('data-datepicker-min'), dateFormat).toDate() || null,
      maxDate: moment(field.getAttribute('data-datepicker-max'), dateFormat).toDate() || null,
      yearRange: [moment(field.getAttribute('data-datepicker-min'), dateFormat).year(), moment(field.getAttribute('data-datepicker-max'), dateFormat).year()],
      showDaysInNextAndPreviousMonths: true
    });

    // add datepicker class
    addClass(field, pickerFieldClass);
    // if datepicker does not have an ID, add one
    if(!field.id){
      field.id = uniqueId();
    }
    // add datepicker label after the field
    var label = document.createElement('label');
    label.setAttribute('for', field.id);
    addClass(label, pickerIconClass);
    field.parentNode.appendChild(label);
  };

  // create global function to get value from any datepicker field
  vl.datepicker.getDate = function(field) {
    return field.value;
  };

  [].forEach.call(pickerFields, function(field) {
    vl.datepicker.dress(field);
  });


})();


/**
 * Show content in drawer
 */
(function () {

  var elements = document.querySelectorAll('[data-drawer]'),
      contentVisibleClass = 'js-drawer__content--visible',
      drawerVisibleClass = 'js-drawer--visible',
      closeClass = 'js-drawer__close',
      smallClass = 'js-drawer--small',
      linkedSliderClass = 'js-drawer--linked-slider',
      activeItemClass = 'js-drawer-active-item',
      defaultItemClass = 'js-drawer-default-item';

  [].forEach.call(elements, function (drawer) {
    dressDrawer(drawer);
  });

  function dressDrawer(drawer) {
    // get all drawer triggers for this specific drawer
    var drawerID = drawer.getAttribute('data-drawer');
    var linkedSlider = hasClass(drawer, linkedSliderClass);
    // add drawer tip
    var tip = document.createElement('div');
    tip.setAttribute('tabindex', -1);
    addClass(tip, 'js-drawer__tip');

    drawer.insertBefore(tip, drawer.firstChild);

    // get every link which should toggle the drawer
    var links = document.querySelectorAll('[data-drawer-id="'+ drawerID +'"]');
    [].forEach.call(links, function (link) {
      // bind click event
      link.addEventListener('click', function (e) {
        e.preventDefault();
        clickLink(link, drawer, tip, true);
      });

      if(hasClass(link, defaultItemClass)) {
        clickLink(link, drawer, tip, false);
      }
    });


    // Close the drawer when clicking close
    var closeLinks = drawer.getElementsByClassName(closeClass);
    [].forEach.call(closeLinks, function (close) {
      close.addEventListener('click', function(e){
        e.preventDefault();
        closeDrawer(drawer);
      });
    });
  }

  function clickLink(link, drawer, tip, scroll) {
    var contentID = link.getAttribute('data-drawer-content-id');
    var drawerContent = drawer.querySelectorAll('[data-drawer-content="'+ contentID +'"]')[0];
    // show drawer content
    if(drawerContent) {
      showDrawerContent(drawerContent, drawer, scroll, tip);
    }
    // make link active + place tip
    activateLink(link, tip, drawer);
  }

  function showDrawerContent(drawerContent, drawer, scroll, tip) {
    var visible = hasClass(drawer, drawerVisibleClass);


    if(visible){
      hideDrawerContent(drawer, false);
    }

    // show drawer
    addClass(drawer, drawerVisibleClass);
    // show content
    addClass(drawerContent, contentVisibleClass);

    if(!visible && scroll) {
      var h = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);
      // scroll to drawer
      var scrollposition = (document.body.scrollTop + drawer.getBoundingClientRect().top) - h/4;
      scrollTo(document.body, scrollposition, 300, function(){ tip.focus(); });
    }else{
      var scrollposition = document.body.scrollTop;
      tip.focus();
      document.body.scrollTop = scrollposition;
    }
  }

  function activateLink(link, tip, drawer) {
    addClass(link, activeItemClass);
    var rect = link.getBoundingClientRect(),
        left = rect.width/2,
        drawerRect = drawer.getBoundingClientRect();
    // get link position
    if(hasClass(drawer, smallClass)) {
      left += (rect.left - drawerRect.left);
    }else{
      left += rect.left;
    }

    tip.style.left = left + 'px';
  }


  function closeDrawer(drawer) {
    removeClass(drawer, drawerVisibleClass);
    hideDrawerContent(drawer, true);
  }

  function hideDrawerContent(drawer, scrollto) {
    var drawerID = drawer.getAttribute('data-drawer');
    // Hide other visible drawer contents
    var visibleContent = drawer.getElementsByClassName(contentVisibleClass)[0];
    if(visibleContent) {
      removeClass(visibleContent, contentVisibleClass);
    }
    // make other links unactive
    var link = document.querySelectorAll('[data-drawer-id="'+ drawerID +'"].' + activeItemClass)[0];
    if(link){
      removeClass(link, activeItemClass);
      if(scrollto) {
        var h = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);
        // scroll to drawer
        var scrollposition = (document.body.scrollTop + link.getBoundingClientRect().top) - h / 4;
        scrollTo(document.body, scrollposition, 300, function () {
          link.focus();
        });
      }
    }
  }
})();

/**
 *
 * Drilldown navigation
 *
**/

vl.drilldown = {};
vl.drilldown.dress;

(function () {

  var elements = document.querySelectorAll('[data-drilldown]'),
      dataColumn = '[data-drilldown-column]',
      column = 'drilldown__column',
      subcolumn = 'drilldown__subcolumn',
      subcolumnHeader = 'drilldown__subcolumn__header',
      activeSubcolumn = 'drilldown__subcolumn--active',
      listWrapper = 'drilldown__list-wrapper',
      item = 'drilldown__item',
      callToAction = 'drilldown__item__cta',
      activeCallToAction = 'drilldown__item__cta--active';

  vl.drilldown.dress = function(drilldown) {

    // Set search
    var hasSearch = false;
    if(drilldown.getAttribute('data-search') !== "false"){
      hasSearch = true;
    }

    // Generate all extra attributes
    generateDrilldownAttributes(drilldown, hasSearch);

    // Set variables
    var callToActions       = drilldown.querySelectorAll('[data-drilldown-cta]');
    var backCallToActions   = drilldown.querySelectorAll('[data-drilldown-back]');
    var coverCallToActions  = drilldown.querySelectorAll('[data-drilldown-cover]');
    var inputFields         = drilldown.querySelectorAll('[data-input]');
    var columns             = parseInt(drilldown.getAttribute('data-drilldown-columns'));

    /*
    * loop General CTA's
    */
    [].forEach.call(callToActions, function (cta) {
      // bind click event
      cta.addEventListener('click', function (e) {
        e.preventDefault();

        // Get state of clicked element
        var isActive = hasClass(cta, activeCallToAction);

        // Get subcolumn of clicked element
        var parentItem = cta.closest('.' + item);
        var child = parentItem.querySelector('.' + subcolumn);

        // Get parent column of clicked element
        var parentColumn = cta.closest(dataColumn);
        removeActiveCtas(parentColumn);
        removeActiveColumns(parentColumn);

        // Toggle the active class based on its active state
        if(!isActive) {
          dressSubColumn(this, child);
          addClass(cta, activeCallToAction);
        }else{
          killSubColumn(this, child);
          removeClass(cta, activeCallToAction);
        }
        triggerResize();

      });
    });

    /*
    * loop Back Buttons
    */
    [].forEach.call(backCallToActions, function (cta) {
      // bind click event
      cta.addEventListener('click', function (e) {
        e.preventDefault();

        // Get subcolumn
        var col = cta.closest('.' + subcolumn);
        killSubColumn(this, col);

        // Get parent's active CTA and remove the active state
        var parent = col.closest('.' + item);
        var activeCta = parent.querySelector('.' + callToAction);
        removeClass(activeCta, activeCallToAction);

        triggerResize();

        return false;
      });
    });

    /*
    * loop Cover CTA's
    */
    [].forEach.call(coverCallToActions, function (cta) {

      // bind click event
      cta.addEventListener('click', function (e) {
        e.preventDefault();

        // Get subcolumn
        var depth = parseInt(cta.getAttribute('data-drilldown-cover'));
        var col = document.querySelector('[data-drilldown-depth="' + (depth +1) + '"]'); // Plus 1 to kill the next column, not the current one

        killSubColumn(this, col);

        // Get parent's active CTA and remove the active state
        var parent = col.closest('.' + item);
        var activeCta = parent.querySelector('.' + callToAction);
        removeClass(activeCta, activeCallToAction);
        triggerResize();
      });
    });

    /*
    * loop data inputs and add search functionality
    */
    [].forEach.call(inputFields, function(inputField){

      var subcol = inputField.closest(dataColumn);
      var depth = subcol.getAttribute('data-drilldown-depth');
      var items = subcol.querySelectorAll('[data-drilldown-index-depth="'+depth+'"]');

      inputField.addEventListener('keyup', function(e){

        if(e.keyCode !== 38 && e.keyCode !== 40 && e.keyCode !== 27 && e.keyCode !== 30 && e.keyCode !== 13){
          var searchString = inputField.value.toLowerCase();
          var k = 0, options = [];
          [].forEach.call(items, function(item){
            var term = stripTags(item.innerHTML.toLowerCase());

            item.removeAttribute('data-index');
            item.removeAttribute('data-focus');
            item.setAttribute('aria-hidden', 'true');

            if(term.indexOf(searchString ) > -1){
              item.setAttribute('data-index', k);
              if(k == 0){
                item.setAttribute('data-focus', 'true');
              }

              options.push(item);
              item.setAttribute('aria-hidden', 'false');
              k++;
            }
          });

          // No results found
          if(options.length <= 0){
            var el = drilldown.querySelector('.drilldown__empty');
            if(el == null){
              var noResultsFoundEl = document.createElement("div");
                  addClass(noResultsFoundEl, 'drilldown__empty');

                  var msg = drilldown.getAttribute('data-search-empty');
                  if(msg == null){
                    msg = "Geen resultaten gevonden";
                  }
                  noResultsFoundEl.innerHTML = msg;

              var wrapper = subcol.querySelector('.' + listWrapper);
              wrapper.appendChild(noResultsFoundEl);
            }
          }else{
            var el = drilldown.querySelector('.drilldown__empty');
            if(el !== null){
              removeElement(el);
            }
          }
        }
      });
    });

    /*
    * dressSubColumn()
    * - Initiates a subcolumn
    * - Sets current columns
    */
    var dressSubColumn = function(cta, subcol){
      // Increase count of columns
      var curCols = parseInt(subcol.getAttribute('data-drilldown-depth'));
      drilldown.setAttribute('data-drilldown-columns', curCols);

      // Add active state of subcolumn
      addClass(subcol, activeSubcolumn);
      subcol.setAttribute('aria-expanded', true);

      // Set focus on first element and (optional) search
      setFocus(subcol);
    }

    /*
    * killSubColumn()
    * Hides a subcolumn
    * - Sets current columns
    */
    var killSubColumn = function(cta, subcol){
      // Decrease count of columns
      var curCols = parseInt(subcol.getAttribute('data-drilldown-depth')) - 1;
      drilldown.setAttribute('data-drilldown-columns', curCols);
      // Remove active state of subcolumn
      removeClass(subcol, activeSubcolumn);
      subcol.setAttribute('aria-expanded', false);

      var activeEls = subcol.querySelectorAll('[data-focus="true"]');
      [].forEach.call(activeEls, function(el){
        el.removeAttribute('data-focus');
      });

      // Set focus on new active col
      var activeCol = subcol.closest('[data-drilldown-column][data-drilldown-depth="'+curCols+'"]');
      setFocus(activeCol);
    }

    /*
    * Drilldown keydown events
    * Progressive enhancement for Drilldown
    * - Arrow keys (up/down) = go up/down the ladder
    * - Escape key = Close current column and go back one step
    * - Enter + Spacebar = Click event on element
    */
    drilldown.addEventListener('keydown', function(e){

      switch(e.keyCode){
        // Escape up
        case 27:
          e.preventDefault();
          var activeColIndex = drilldown.getAttribute('data-drilldown-columns');
          var activeItem = drilldown.querySelector('[data-drilldown-depth="' + activeColIndex + '"] [data-focus="true"]');
          if(activeItem !== null){
            var col = activeItem.closest('.drilldown__subcolumn');
            if(col !== null){
              var closeButton = col.querySelector('[data-drilldown-back]');
              closeButton.click();
            }
          }
        break;

        // Arrow up
        case 38:
          e.preventDefault();
          moveFocus("up");
        break;

        // Arrow down
        case 40:
          e.preventDefault();
          moveFocus("down");
        break;

        // Enter / Spacebar
        case 13:
          var activeColIndex = drilldown.getAttribute('data-drilldown-columns');
          var activeItem = drilldown.querySelector('[data-drilldown-depth="' + activeColIndex + '"] [data-focus="true"]');
          if(activeItem !== null){
            e.preventDefault();
            activeItem.click();
          }
        break;
      }
    });

    var setFocus = function(col){
      // Set timeout necessary to wait for end of animation
      window.setTimeout(function(){
        var activeEl = col.querySelector('[data-focus="true"]');
        if(activeEl == null){
          col.querySelector('[data-index="0"]').focus();
          col.querySelector('[data-index="0"]').setAttribute('data-focus', 'true');
        }else{
          activeEl.focus();
        }

        if(hasClass(col, 'drilldown__subcolumn') && hasSearch){
          col.querySelector('[data-input]').focus();
        }
      }, 200);
    }

    var moveFocus = function(direction){
      var activeColIndex = drilldown.getAttribute('data-drilldown-columns');

      var activeItem = drilldown.querySelector('[data-drilldown-depth="' + activeColIndex + '"] [data-focus="true"]');
      if(activeItem !== null){

        var curIndex = activeItem.getAttribute('data-index');
        var colItems = drilldown.querySelectorAll('[data-drilldown-index-depth="'+ activeColIndex +'"][aria-hidden="false"]');

        switch(direction){
          case "up":
            if(curIndex > 0){
              curIndex --;
              _setActiveStateAndFocus();
            }
          break;

          case "down":
          //  Checken als cuindex kleiner is dan aantal items in de kolom
            if(curIndex < (colItems.length - 1)){
              curIndex ++;
              _setActiveStateAndFocus();
            }
          break;
        }
      }


      function _setActiveStateAndFocus(){
        activeItem.removeAttribute('data-focus');
        var newItem = drilldown.querySelector('[data-drilldown-index-depth="' + activeColIndex + '"][data-index="' + curIndex + '"]');
        newItem.focus();
        if(hasSearch){
          var searchInput = drilldown.querySelector('[data-drilldown-depth="' + activeColIndex + '"] [data-input]');
          searchInput.focus();
        }
        newItem.setAttribute('data-focus','true');
      }
    }

    /*
    * removeActiveCtas()
    * Removes all active CTA classes from CTA's starting from parentNode downwards
    */
    var removeActiveCtas = function(parentColumn){
      var activeCtas = parentColumn.querySelectorAll('.' + activeCallToAction);
      if(activeCtas !== null){
        [].forEach.call(activeCtas, function(e){
          removeClass(e, activeCallToAction);
        });
      }
    }

    /*
    * removeActiveColumns()
    * Removes all active Column classes from columns starting from parentNode downwards
    */
    var removeActiveColumns = function(parentColumn){
      var els = parentColumn.querySelectorAll(dataColumn);

      if(els !== null){
        [].forEach.call(els, function(e){
          if(hasClass(e, activeSubcolumn)){
            removeClass(e, activeSubcolumn);
          }
        });
      }
    }

    /*
    * triggerResize()
    * Invokes a resize event so equalheight is recalculated
    */
    var triggerResize = function(){
        var cols = drilldown.querySelectorAll('.' + subcolumn);
        var maincol = drilldown.querySelector('.' + column);
        var maxHeight = "";
        maincol.style.height = "";

        // Get max height
        [].forEach.call(cols, function(col){
          // Variables
          var wrapper = col.querySelector('.' + listWrapper);
          var header = col.querySelector('.' + subcolumnHeader);
          // Reset all
          wrapper.style.height = "";
          col.style.height = "";
          // Get height
          var colHeight = col.offsetHeight;
          // Check if height exceeds windowheight
          if(colHeight > maxHeight){
            if(colHeight > (window.innerHeight/1.5)){
              maxHeight = window.innerHeight/1.5;
            }else{
              maxHeight = colHeight;
            }
          }
          // Apply height to wrapper
          wrapper.style.height = (maxHeight - header.offsetHeight) + "px";
        });

        // Apply max height on all subcols
        [].forEach.call(cols, function(col){
          col.style.height = maxHeight + "px";
        });
        // Apply max height on first col
        maincol.style.height = maxHeight + "px";
    }

    /*
    * generateDrilldownAttributes();
    * Generate extra attributes through JS
    */
    function generateDrilldownAttributes(drilldown, hasSearch){

      // Drilldown
      drilldown.setAttribute('data-drilldown-columns', '1');

      // Drilldown cols
      var cols = drilldown.querySelectorAll('.drilldown__column, .drilldown__subcolumn');

      var first = false;
      [].forEach.call(cols, function(col){
        col.setAttribute('data-drilldown-column','');
        var depth = col.getAttribute('data-drilldown-depth');

        // Generate cover (go back) cta
        _generateCoverCta(col, depth);

        // Generate back cta (arrow)
        _generateBackCta(col);

        // Generate search
        if(hasClass(col, 'drilldown__subcolumn') && hasSearch){
          _generateSearch(drilldown, col);
        }

        // Generate data indexes on CTA's
        var k = 0;
        var list = col.querySelector('.drilldown__list');
        [].forEach.call(list.querySelectorAll(':scope > .drilldown__item > .drilldown__item__cta'), function(cta){
          cta.setAttribute('data-drilldown-index-depth', depth);
          cta.setAttribute('data-index', k);
          cta.setAttribute('aria-hidden', 'false');
          k++;
        });
      });


      function _generateCoverCta(col, depth){
        var coverCTA = document.createElement("button");
          coverCTA.setAttribute("type","button");
          coverCTA.setAttribute("data-drilldown-cover",depth);
          addClass(coverCTA, "drilldown__cta-cover");

          var coverCTASpan = document.createElement("span");
              coverCTASpan.innerHTML = "Terug naar niveau " + depth;

          coverCTA.appendChild(coverCTASpan);
        col.appendChild(coverCTA);
      }

      function _generateBackCta(col){
        var backCTA = document.createElement("button");
          backCTA.setAttribute("type","button");
          backCTA.setAttribute("data-drilldown-back","");
          addClass(backCTA, "drilldown__item__cta--back");

          var backCTASpan = document.createElement("span");
              backCTASpan.innerHTML = "Sluit submenu";

          backCTA.appendChild(backCTASpan);

        var header = col.querySelector('.' + subcolumnHeader);
        if(header !== null)
            header.appendChild(backCTA);
      }

      function _generateSearch(drilldown, col){

        var header = col.querySelector('.' + subcolumnHeader);
        if(header !== null){
          var placeholder = drilldown.getAttribute('data-search-ph');
          if(placeholder == null){
            placeholder = "Zoek";
          }
          var inputWrapper = document.createElement('div');
            addClass(inputWrapper, 'drilldown__subcolumn__search');

            var input = document.createElement('input');
                addClass(input, 'drilldown__subcolumn__search__input');
                addClass(input, 'input-field');
                addClass(input, 'input-field--block');
                input.setAttribute('type', 'text');
                input.setAttribute('aria-label', placeholder);
                input.setAttribute('placeholder', placeholder);
                input.setAttribute('data-input', '');

                inputWrapper.appendChild(input);

            header.appendChild(inputWrapper);
        }else{
          // Generate warning
          console.warn("Search warning in subcolumn " + col.getAttribute('data-drilldown-depth') + ": " + "In order to apply the search bar make sure a subcolumn header (.drilldown__subcolumn__header) is present inside the subcolumn.");
        }
      }
    }
  };

  [].forEach.call(elements, function (drilldown) {
    vl.drilldown.dress(drilldown);
  });

})();


vl.initGoogleMaps;

(function() {

  /**
   * Add all eventlisteners to window
   */

  /**
   * Initiate the google map
   */
  vl.initGoogleMaps = function() {

    var elements = document.querySelectorAll('.map');

    [].forEach.call(elements, function(map) {
      var interactiveMap = map.getElementsByClassName('js-interactive-card')[0];
      if(typeof interactiveMap === 'undefined') {
        return;
      }
      var latLng = new google.maps.LatLng(interactiveMap.getAttribute('data-lat'), interactiveMap.getAttribute('data-lng'));
      var gmapOptions = {
        center: latLng,
        zoom: 15,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        disableDefaultUI: true,
        zoomControl: true,
        scrollwheel: false,
        styles: getMapStyling()
      };

      var googleMap = new google.maps.Map(interactiveMap, gmapOptions);

      var image = {
        url: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADgAAABQCAYAAABMIbYpAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAACKpJREFUeNrdnAlQU0cYxxMIEEheAklQwaO2ojNab0WrtdrL4lGtVqfUc6rjOBVbx+pU1NHqeIxVax10rEcRr2I9Ae8TLzxQvFDxRDzw1opi1arg9vuStzE+8iDZ9/ISfTP/GQd23+6Pb9/ut99+q0ol07NusSEc9A3oV9B60GnQDdATEFm7yFAAyktJMmQsm8Mlzp2iH9S/p7YRVC0PsoCMoGCQWuUrD3S8EmgY6AjoJYK4q9XzDacBdmq7zwKbwytrgKqDKoHCQBpvgdUFLQUVOXY2a3MoubDfRG6fMJPCCxby9JKFFOWHE3IjnDy/Ek7+zbWQu6fMJO+giRzdGkq2LTfa64J1nyfP4lL7fKv9ggeligRplQKzgJIcrXVkSyi5lW0mL6/bQNzVLfhjHIY/jANo0aIZXHLThprGAtCKoEBPwnUC3acdObEjjDzJszBBOdPjixZyfHuYHTRtgeHmiEHB3QWQOHzNsn6n0Jg/aBq1WtamUOtQkwtMqIfnLeTQRrtFixKn6RM4vbqmALSKLN8nNBAISnW0mqfAhMpOf2VNmHW3Vq3sV0cAWU3St8nDrcMGtsJkUHDWohgc1R2YkLYus01EK+Zx+ypH+tUWQEaBQlgBF+OLt68wenRIliVse8cqI7VkupPhGuW2JeFlP+ILNyUbvArnCEmXlKTp+jkCQDpcNa7CNQY9w5ddP2b2OhzVP6fN9Jt8OX5YSD8nkFXKnF2hchAoB1+UvT3MZ+CocJLjl5A7DetoGjqBNJcFOAZfsH9dqM/BUR3kl5CFCdxCJ4DVRZ0BqBQBeoyV0Z3yVcAHMJtT167rl0EfO4GMFAOchRUPb/Fd61GhH4t9BZdusRNAVJAQzkS3NehJ+Drgg3N2KxbWquFfzwlgBSHgEKxwaJN066HTvSfVSIYNCCIxrfxI/Voq0uB9FWn7iT+JHxhEdqcYmR1zR2VusFkxYYJuhMi36OcImIWFbx6X9u1dzTKRXl0CrFClqVsnDbl4wCStrcMm6uFkigxTA4WrQj0WKQ3ilqdlU3WZcFQtotUkc71R0kihDnnz6IDGTgAjKGAvLIgfLmtjeZkm8mFj1+GoPmigJuf3sa+3+Elh3yeN1A0Q8W6sgH9gocuH2IdM39gAt+GousNwLWb8JjF6wLtvc0WGaSAC7pCy9mXAhMIKR7VrNdtQxdAI74TvFAHkEPAyFsIdNUsjw38IkgwYHxfE1DbGfbDvKUmGXBFAEwI+wELPr7INkw6t/SUDtv/Un6ntZ1dsgGsWGgpEAMsjYDEWYl2bouuqJAM2qaeWPJOKAEaoqAdDw3vuCqd7qYDoCLC0XXwt3O6XigBWRMC7WOjZZbZv8Ou20ocoejwsbf932T5E75ZmwZNYqJDRB50YH+y1SeZRrn2SyRMBrICAa7AQBm9ZGsEwolRAXGqYdvk5tmVi5Z9clgigWcUfllgXTdaFvn8P9oW+d9cA5gkOnRPs+5KZ3N+i/igU6EwDuqyA2BDLZIN1Lh1k/8PSSDjsKEaW5smUk7pU0E3ox838XIZrBY75EYmb6518OLFvN22MqC/K+6OZ1u/whLTtUj5sYVzxS3FYYlk5omypSYYrojOoA2C81B3Fa4Eh2IyOHaIlnWL8SZP6atKwtoq0A29l7FAt2b9Wng0vDe2X4mhzjoCV6Vkfq8umpNApoTHSfj20bZ3ARZWIkdJziPP7TD4PeG6vbfZcPpfLELFeOWdRtRislJFm9HnAjDW2je6YoSHficRjNM4A1aDDWDH3gO9aEfvGn+2fctl6DpAdsfKeVN+14p4029IwcXjI9yLfnqY0QDVdMnCc+xoczg/Yt1WJXLaI9cJcOV1qjS/ZuVqe6VxO7UqxWW/csJC+IqdLLp8P7sQXnd7tOydMZzPCSnOsq5cI15cB2Bxflr7SSF7kex8ON7b0lHfU4JCebh+biUBuVDrxQEw4kvgo9n4ncO8wpZXwJ73W1BEM7ngLDkcQjiTsx89xwbFOACVlWqR524r0VHfZHG63E7hwOXLSrFbEvDOl4WhYEPvwU//gzgK4qrJkPMHLl1vP7NOVt+KJdLv1tjmxXrBcqVw16YaYNfrNIhwxvPWKB/bRdnDZHWOEXIKNHd+ufCrX0tncBgHce68dbsoEGAV6gQ0qkRiEI4VGrPt117YRAOo8lU4537rr3+Z5Kx7bZrNe8iwurdSzd5kBq9IMqEIPJio8umBPMCjq2SXoc8HQ9Pd0xu9smuXr6RSRJTO55QLr6ZVIaa4IeoodwFQO2ZNhX6WHPI/96rUknwglE9ITrAlDm+W3Io4MPsHnL6ZMQpkAy9OUr/tn5Ev5KniVovW0Y0xgixLpIApfK5giNdxf4hBnsz3JboEg094r9ybwekEhduhejnQr0ig1WO9J65YBzVyKrygAOUGu9C+a7yKIUod6+1pPGKgAO3bnJLsVMX2Ft96jj5oGRPNwlX3l7tIo7BwmqUpNcE2cpp9ZZlKrFwANoHusJ8S3T5rpGfvD6PqaRi6H/hSGxNtn1vRG1pTIeVP1v7sd+lMQUAe6hR294UY65s1su/Xu1ampqe926E9hyMFWK6533YpYFuvMmayfzBz6UxAwGHQNO3ztaNlWxPsY9HpAtar+dZlDfwpDxmGnD7hgxQO89WZN0o+XHPpTEDCQZi7mHxE/uMHf8da7wV+2Cle9KQ90vF9ZF0vwd1hmxgTdaKjyrs8PTQGgBpSLAFeySloRf8ZnRuSHm/1qyRb6Uxiyl5gVqfWmj9PFq2zXy1VvIiBeiT2DII5ZTJf4tCtMnLOY/KrLHvpTGDIWYfatfWVF/Df+7LdfdEM8FvpTENAPdgfWNE28AHIx0544cN7AqSNVb8PDX0u3pqXs5dM+pozWxXk89KcgoBrWumMO/6VDDqdTG1Rv0wOOdHsKmDBe11v1Nj6w5mWtSuSOeTW+4sln6WyuDezWO3qr/f8BVZY5l/q5m5EAAAAASUVORK5CYII=',
        size: new google.maps.Size(56, 80),
        origin: new google.maps.Point(0, 0),
        anchor: new google.maps.Point(14, 40),
        scaledSize: new google.maps.Size(28, 40)
      };
      var marker = new google.maps.Marker({
        position: latLng,
        map: googleMap,
        title: '',
        icon: image
      });
      google.maps.event.trigger(googleMap, 'resize');
      googleMap.setCenter(marker.getPosition());

      // add overlay over the map to prevent scroll
      var swipePreventer = document.createElement('div');
      addClass(swipePreventer, 'map__swipe-preventer');
      insertAfter(swipePreventer, interactiveMap);

      // hide the swipe-preventer on hover (mouse users) and on click (touch users)
      swipePreventer.addEventListener("mouseover", function(){
        this.style.display = 'none';
      });
      swipePreventer.addEventListener("click", function(){
        this.style.display = 'none';
      });
    });


    /**
     * Returns the default map styling.
     */
    function getMapStyling() {
      var $mapStyle = [
        {
          featureType: 'administrative',
          elementType: 'all',
          stylers: [
            { visibility: 'off' }
          ]
        },
        {
          featureType: 'administrative.country',
          elementType: 'geometry.stroke',
          stylers: [
            { visibility: 'on' },
            { lightness: '50' }
          ]
        },
        {
          featureType: 'administrative.land_parcel',
          elementType: 'all',
          stylers: [
            { hue: '#ff0000' }
          ]
        },
        {
          featureType: 'landscape',
          elementType: 'all',
          stylers: [
            { visibility: 'on' },
            { color: '#ebecef' }
          ]
        },
        {
          featureType: 'poi',
          elementType: 'all',
          stylers: [
            { visibility: 'off' }
          ]
        },
        {
          featureType: 'road',
          elementType: 'all',
          stylers: [
            { saturation: -100 },
            { lightness: '50' },
            { visibility: 'simplified' },
            { gamma: '1' }
          ]
        },
        {
          featureType: 'road',
          elementType: 'geometry',
          stylers: [
            { visibility: 'on' }
          ]
        },
        {
          featureType: 'road',
          elementType: 'labels',
          stylers: [
            { visibility: 'on' }
          ]
        },
        {
          featureType: 'road',
          elementType: 'labels.text',
          stylers: [
            { gamma: '0.54' }
          ]
        },
        {
          featureType: 'road',
          elementType: 'labels.icon',
          stylers: [
            { visibility: 'off' }
          ]
        },
        {
          featureType: 'road.highway',
          elementType: 'all',
          stylers: [
            { visibility: 'simplified' }
          ]
        },
        {
          featureType: 'road.highway',
          elementType: 'geometry',
          stylers: [
            { visibility: 'simplified' },
            { color: '#cbd2da' }
          ]
        },
        {
          featureType: 'road.highway',
          elementType: 'labels',
          stylers: [
            { lightness: '50' },
            { gamma: '1' }
          ]
        },
        {
          featureType: 'road.highway',
          elementType: 'labels.icon',
          stylers: [
            { visibility: 'off' }
          ]
        },
        {
          featureType: 'road.highway.controlled_access',
          elementType: 'all',
          stylers: [
            { visibility: 'simplified' }
          ]
        },
        {
          featureType: 'road.highway.controlled_access',
          elementType: 'labels.icon',
          stylers: [
            { visibility: 'off' }
          ]
        },
        {
          featureType: 'road.arterial',
          elementType: 'geometry.stroke',
          stylers: [
            { color: '#cbd2da' }
          ]
        },
        {
          featureType: 'road.local',
          elementType: 'all',
          stylers: [
            { visibility: 'on' }
          ]
        },
        {
          featureType: 'road.local',
          elementType: 'geometry.stroke',
          stylers: [
            { gamma: '7.17' },
            { color: '#cbd2da'}
          ]
        },
        {
          featureType: 'transit',
          elementType: 'all',
          stylers: [
            { visibility: 'off' }
          ]
        },
        {
          featureType: 'water',
          elementType: 'all',
          stylers: [
            { color: '#bfdfe9' },
            { visibility: 'simplified' }
          ]
        },
        {
          featureType: 'water',
          elementType: 'geometry.fill',
          stylers: [
            { visibility: 'on' }
          ]
        },
        {
          featureType: 'water',
          elementType: 'geometry.stroke',
          stylers: [
            { visibility: 'off' }
          ]
        },
        {
          featureType: 'water',
          elementType: 'labels',
          stylers: [
            { visibility: 'off' }
          ]
        }
      ];

      return $mapStyle;
    }
  };

  // if maps is already loaded
  if (typeof google === 'object' && typeof google.maps === 'object') {
    vl.initGoogleMaps();
  }else{
    window.addEventListener('googlemaps_loaded', vl.initGoogleMaps, false);
  }
})();



// public function so it can be used as callback when google maps is loaded
function initGoogleMaps() {
  vl.initGoogleMaps();
}


/**
 * Show container when specific option is selected in select attribute
 */
(function () {

  var wrappers = document.querySelectorAll('[data-show-on-select-wrapper]');

  [].forEach.call(wrappers, function (soswrapper) {
    dressShowOnSelect(soswrapper);
  });

  function dressShowOnSelect(soswrapper) {

    // get all selects in wrapper
    var sosSelects = soswrapper.querySelectorAll('[data-show-on-select]');

    // Loop through selects in wrapper
    [].forEach.call(sosSelects, function(select){

      // detect change in select elem
      select.onchange = function(e){
        var dataId = select.getAttribute('data-id');
        var selectedOpt = select.options[select.selectedIndex];

        if(selectedOpt.hasAttribute('data-show-on-select-option')){
          resetContent(dataId, soswrapper);

          if(selectedOpt.hasAttribute('data-target')){
            var dataTarget = selectedOpt.getAttribute('data-target');
            showContent(dataTarget, soswrapper);
          }
        }
      };

    });
  }

   /*
    * resetContent()
    * dataId: dataId of selected option
    */
    function resetContent(dataId, soswrapper){
      //  select all contentblocks based on their data-id attribute
      var contentblocks = soswrapper.querySelectorAll('[data-show-on-select-content][data-id="'+ dataId +'"]');

      // Loop through content blocks
      [].forEach.call(contentblocks, function(elem){
        // Set attributes & tabindex
        elem.setAttribute('aria-hidden','true');
        elem.setAttribute('data-show','false');
        elem.setAttribute('tabindex','-1');
      });
    }

   /*
    * showContent()
    * dataTarget: dataTarget of selected option
    */
    function showContent(dataTarget, soswrapper){
      //  select contentblock based on their data-hook attribute
      var contentblock = soswrapper.querySelector('[data-show-on-select-content][data-hook="'+ dataTarget +'"]');

      // Set attributes & tabindex
      contentblock.setAttribute('aria-hidden','false');
      contentblock.setAttribute('data-show','true');
      if(contentblock.hasAttribute('tabindex'))
        contentblock.removeAttribute('tabindex');

    }



})();

/**
 * Dress the sticky elements. Mostly used for side navigation
 * An element is sticky in its closest parent .layout
 * If layout is not present, which should be, we take .region
 */
(function () {

  var absoluteClass = 'js-sticky--absolute';
  var fixedClass = 'js-sticky--fixed';
  var elements = document.getElementsByClassName('js-sticky');
  var latestKnownScrollY = 0,
      ticking = false,
      resized = true;

  // prepare the elements by wrapping in placeholder
  function initiateStickyElement(stickyContent) {

      // set the original position on load + add parent if required
      storeElementData(stickyContent);

      // put placeholder around the sticky content
      var placeholder = document.createElement('div');
      addClass(placeholder, 'js-sticky-placeholder');
      wrap(stickyContent, placeholder);
      // set placeholder height fixed
      placeholder.style.height = placeholder.offsetHeight + "px";
  }

  function storeElementData(stickyContent) {
    removeClass(stickyContent, fixedClass);
    removeClass(stickyContent, absoluteClass);

    // set position properties
    stickyContent.style.top = 'auto';
    stickyContent.style.bottom = 'auto';
    stickyContent.style.left = 'auto';
    stickyContent.style.width = 'auto';

    var rect = stickyContent.getBoundingClientRect();
    var offsetTop = rect.top + window.pageYOffset;
    stickyContent.setAttribute('data-original-top', offsetTop);
    var offsetLeft = rect.left + window.pageXOffset;
    stickyContent.setAttribute('data-original-left', offsetLeft);
    stickyContent.setAttribute('data-original-width', rect.width);
    var stickyContainer = stickyContent.closest('.layout, .region');
    stickyContent.setAttribute('data-max-y', stickyContainer.getBoundingClientRect().bottom + window.pageYOffset);
  }

  function makeSticky(element, offset, scrollPosition) {
    var placeholder = element.parentElement;

    // if the user has scrolled passed the start + offset of the element, make it sticky or update its sticky position
    if(scrollPosition + offset >= element.getAttribute('data-original-top')) {
      var elementHeight = element.offsetHeight;
      // If the element bottom is going outside of the container, make it absolute
      if(element.getAttribute('data-max-y') > elementHeight && element.getAttribute('data-max-y') - scrollPosition - offset < elementHeight) {
        if(!hasClass(element, absoluteClass)){
          // set position properties
          element.style.top = 'auto';
          element.style.bottom = (parseInt(element.getAttribute('data-original-top')) + elementHeight - element.getAttribute('data-max-y')) + 'px'; // calc position t.o.v. placeholder
          element.style.left = '0px';
          element.style.width = '100%';
        }
        addClass(element, absoluteClass);
        removeClass(element, fixedClass);
      } else if(!hasClass(element, fixedClass)) {
        addClass(element, fixedClass);
        removeClass(element, absoluteClass);
        // set position properties
        element.style.top = offset + 'px';
        element.style.bottom = 'auto';
        element.style.left = element.getAttribute('data-original-left') + 'px';
        element.style.width = element.getAttribute('data-original-width') + 'px';
      }
    } else if(hasClass(element, absoluteClass) || hasClass(element, fixedClass)) {
      removeClass(element, fixedClass);
      removeClass(element, absoluteClass);

      // set position properties
      element.style.top = 'auto';
      element.style.bottom = 'auto';
      element.style.left = 'auto';
      element.style.width = 'auto';
    }
  }

  // only add sticky if all content is loaded
  window.addEventListener('load', function() {

    // only on medium or up
    if(vl.breakpoint.value == 'small' || vl.breakpoint.value == 'xsmall'){
      return;
    }

    [].forEach.call(elements, function (stickyContent) {
      initiateStickyElement(stickyContent);
    });

    window.addEventListener('scroll', onScroll, false);
    window.addEventListener('resize', refresh, false);

    onScroll();
  });

  var refresh = function(){
    if(resized) {
      requestAnimationFrame(resetElements);
    }
    resized = false;
  };

  function resetElements () {
    resized = true;

    [].forEach.call(elements, function(el) {
      storeElementData(el);
      onScroll();
    });
  }

  function onScroll() {
    latestKnownScrollY = window.pageYOffset;
    requestTick();
  }

  function requestTick() {
    if(!ticking) {
      requestAnimationFrame(update);
    }
    ticking = true;
  }


  function update() {
    ticking = false;
    var currentScrollY = latestKnownScrollY;

    [].forEach.call(elements, function(el) {
      makeSticky(el, 75, currentScrollY);
    });
  }

})();

// To do: optimize
// - detect content changes

/**
 * We assume that in a sticky element items with an anchor link should have a scrollspy functionality
 */

(function () {

  // find all #-links in sticky elements
  var elements = document.querySelectorAll('.js-sticky a[href^="#"]'),
      latestKnownScrollY = 0,
      ticking = false;


  // only add scrollspy if all content is loaded
  window.addEventListener('load', function() {
    
    // only on medium or up
    if (vl.breakpoint.value == 'small' || vl.breakpoint.value == 'xsmall') {
      return;
    }

    window.addEventListener('scroll', onScroll, false);
    //window.addEventListener('resize', refresh, false);

    onScroll();
  });

  function onScroll() {
    latestKnownScrollY = window.pageYOffset;
    requestTick();
  }

  function requestTick() {
    if(!ticking) {
      requestAnimationFrame(update);
    }
    ticking = true;
  }

  function update() {
    ticking = false;
    var currentScrollY = latestKnownScrollY;

    [].forEach.call(elements, function(el) {
      checkScrollSpy(el);
    });
  }

  function checkScrollSpy (element) {
    var href = element.getAttribute('href');
    var target = document.querySelector(href);
    var h = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);
    if(target && target.getBoundingClientRect().top < h/2){
      setActiveItem(element);
    }
  }

  function setActiveItem(item) {
    var activeClass = 'js-scrollspy-active';
    var otherItems = document.getElementsByClassName(activeClass);
    [].forEach.call(otherItems, function(el) {
      if(item !== el){
        removeClass(el, activeClass);
      }
    });
    addClass(item, activeClass);
  }


})();

/**
 * Fix a range slider for the range element
 */
(function () {

  var elements = document.getElementsByClassName('js-range');

  [].forEach.call(elements, function (element) {
    var slider = element.getElementsByClassName('js-range__slider')[0],
        fromInput = element.getElementsByClassName('js-range__from')[0],
        toInput = element.getElementsByClassName('js-range__to')[0];
    noUiSlider.create(slider, {
      start: [fromInput.value, toInput.value],
      step: 1,
      connect: true,
      range: {
        'min': parseInt(fromInput.value),
        'max': parseInt(toInput.value)
      }
    });

    // When the slider value changes, update the input and span
    slider.noUiSlider.on('update', function( values, handle ) {
      fromInput.value = parseInt(values[0]);
      toInput.value = parseInt(values[1]);
    });

    fromInput.addEventListener('change', function(){
      slider.noUiSlider.set([this.value, null]);
    });

    toInput.addEventListener('change', function(){
      slider.noUiSlider.set([null, this.value]);
    });
  });

})();

/**
 * Push a fixed element by another element in viewport
 * Written to push helpwidget by global footer
 */
(function () {

  var elements = document.getElementsByClassName('js-push'),
      ticking = false;

  function onScroll() {
    requestTick();
  }

  function requestTick() {
    if(!ticking) {
      requestAnimationFrame(update);
    }
    ticking = true;
  }

  function update() {
    // reset the tick so we can
    // capture the next onScroll
    ticking = false;

    [].forEach.call(elements, function (element) {
      var pusher = document.getElementsByClassName(element.getAttribute('data-push-by'))[0],
          spacer = parseInt(element.getAttribute('data-push-space')) || 0,
          space = Math.min(0, pusher.getBoundingClientRect().top - element.getBoundingClientRect().bottom - spacer -  Number(element.style.marginBottom.replace(/[^\d\.\-]/g, ''))) * -1 ;

      element.style.marginBottom = space + 'px';
    });
  }

  // add event listener
  if(elements.length) {
    window.addEventListener('scroll', onScroll, false);
  }

})();


/**
 * Set some elements to the same height
 */
(function () {

  function sameHeights (container) {

    var nodeList = container.getElementsByClassName('js-equal-height');
    var elems = [].slice.call(nodeList);

    var tallest = Math.max.apply(Math, elems.map(function(elem, index) {
      elem.style.minHeight = ''; // clean first
      elem.style.height = ''; // clean first
      return elem.offsetHeight;
    }));

    tallest ++;

    elems.forEach(function(elem, index, arr){
      if(elem.hasAttribute('data-equal-height') && elem.getAttribute('data-equal-height') == "height"){
        elem.style.height = tallest + 'px';
      }else{
        elem.style.minHeight = tallest + 'px';
      }
    });

  }

  var resized = true;

  function fixContainers() {
    resized = true;

    // apply per container
    var containers = document.getElementsByClassName('js-equal-height-container');

    [].forEach.call(containers, function (container) {
      sameHeights(container);
    });
  }

  var refresh = function(){
    if(resized) {
      requestAnimationFrame(fixContainers);
    }
    resized = false;
  };
  // only add sticky if all content is loaded
  window.addEventListener('load', function() {
    window.addEventListener('resize', refresh, false);
    refresh();
  });
})();

/**
 * Toggle the small class for the bar-navigation if possible
 */
(function () {

  var elements = document.getElementsByClassName('js-bar-navigation');

  // store the width at which the bar should no longer be small
  [].forEach.call(elements, function (element) {
    if(element.offsetHeight < 80) {
      var bar = element.getElementsByClassName('js-bar-content')[0];
      element.setAttribute('data-bar-width', bar.offsetWidth);
    }
  });

  function checkBarNavigation() {
    [].forEach.call(elements, function (element) {
      var resetWidth = element.getAttribute('data-bar-width');
      // a bar navigation wraps over 2 lines if it is 80px or higher
      if(element.offsetHeight >= 80){
        addClass(element, 'js-bar-navigation--small');
      }else if(element.offsetWidth > resetWidth && resetWidth > 0) {
        removeClass(element, 'js-bar-navigation--small');
      }
    });
  }

  // initiate
  checkBarNavigation();

  window.addEventListener('resize', function() {
    checkBarNavigation();
  });

})();

/**
 * Create a tag input by data-attribute
 */

vl.dressTaggle;

(function () {

  var elements = document.getElementsByClassName('js-tag-input');



  vl.dressTaggle = function(field) {
    // add div to show tag input
    var tagDiv = document.createElement('div');
    insertAfter(tagDiv, field);
    addClass(tagDiv, 'tag-input-wrapper');
    addClass(tagDiv, 'input-field');
    addClass(tagDiv, 'input-field--block');

    var name = field.name;
    var placeholder = field.placeholder;

    var taggleSettings = {
      containerFocusClass: 'input-field--focus',
      hiddenInputName: name + '[]',
      placeholder: placeholder,
      tabIndex: 0,
      duplicateTagClass: 'taggle--double'
    };

    removeElement(field);

    new Taggle(tagDiv, taggleSettings);
  };



  [].forEach.call(elements, function (element) {
    vl.dressTaggle(element);
  });

})();

/**
 * Bind the dynamic input
 */
(function () {

  var elements = document.getElementsByClassName('js-dynamic-label');

  [].forEach.call(elements, function (element) {
    // get all variables
    var label = element.getElementsByClassName('js-dynamic-label__value')[0],
        value = label.innerHTML,
        input = element.getElementsByClassName('js-dynamic-label__field')[0],
        toggle = element.getElementsByClassName('js-dynamic-label__toggle')[0];

    // set input value
    input.value = value;

    // add toggle event on clicking the icon
    toggle.addEventListener('click', toggleInput);
    // toggle on esc / enter in the input field
    input.addEventListener('keydown', function(e){
      if (e.keyCode === 13) {
        e.preventDefault();
        // move focus to
        fireEvent(input, 'blur');
        // check if it is valid
        if(input.getAttribute('data-has-error') !== "true"){
          // On enter transfer the value
          label.innerHTML = this.value;
          toggleInput();
        }
      }
      // on esc
      if(e.keyCode === 27) {
        e.preventDefault();
        this.value = label.innerHTML;
        toggleInput();
      }
    });

    // function to activate the input
    function toggleInput() {
      var activeClass = 'dynamic-label--active';

      if(hasClass(element, activeClass)){
        removeClass(element, activeClass);
      }else{
        addClass(element, activeClass);
        input.select();
      }
    }
  });

})();

/**
 * Simulate float-right functionality for a col element
 */

vl.colFloatRight;
(function () {

  if(!Modernizr.flexbox){
    return;
  }

  var elements = document.getElementsByClassName('js-col-float-right');

  vl.colFloatRight = function (col) {
    var colRect = col.getBoundingClientRect();
    // the intersecting col elements
    // get all col elements
    var cols = document.querySelectorAll('.grid > *');
    [].forEach.call(cols, function (element) {
      // check if the element is not inside a floating col
      var parent = getParentsUntil(element, 'js-col-float-right');
      var rect = element.getBoundingClientRect();
      if(element !== col && parent == null && rect.right > colRect.left && rect.top < colRect.bottom && rect.bottom > colRect.top){
        // add margin to the parent
        var grid = element.closest('.grid');
        var margin = colRect.bottom - rect.top;
        grid.style.marginTop = parseInt(margin)+'px';
        addClass(grid, 'js-col-float-right--pushed');
      }
    });
  };


  if(vl.breakpoint.value !== 'small' && vl.breakpoint.value !== 'xsmall') {
    [].forEach.call(elements, function (element) {
      vl.colFloatRight(element);
    });
  }

  /**
   * Add eventlisteners to window
   */
  window.addEventListener('resize', function() {
    if(vl.breakpoint.value !== 'small' && vl.breakpoint.value !== 'xsmall') {
      [].forEach.call(elements, function (element) {
        vl.colFloatRight(element);
      });
    }
  });

})();

/**
 * Rotate the lines according to the scroll position
 */
(function () {

  var bannercontent = document.getElementsByClassName('js-banner__content')[0];

  if (!bannercontent){
    return;
  }

  var header = bannercontent.closest('.banner'),
    line2 = header.getElementsByClassName('js-banner__line-2')[0],
    line3 = header.getElementsByClassName('js-banner__line-3')[0],
    headerheight = header.getBoundingClientRect().height,
    latestKnownScrollY = window.pageYOffset,
    ticking = false,
    // randomly rotate the lines by default
    line2rotate = randomIntFromInterval(10, 20),
    line3rotate = randomIntFromInterval(25, 40);

  if (!header || !line2 || !line3) {
    return;
  }

  function onScroll() {
    latestKnownScrollY = window.pageYOffset;
    requestTick();
  }

  function requestTick() {
    if (!ticking) {
      requestAnimationFrame(update);
    }
    ticking = true;
  }

  function update() {

    // reset the tick so we can
    // capture the next onScroll
    ticking = false;

    var percentage = (latestKnownScrollY / headerheight);
    // rotate lines according to scrolled height
    var line2tr = percentage * line2rotate;
    line2.style['transform'] = "rotate(" + line2tr + "deg) translateY(" + Math.trunc(latestKnownScrollY / 2) + "px)";
    var line3tr = percentage * line3rotate;
    line3.style['transform'] = "rotate(" + line3tr + "deg) translateY(" + Math.trunc(latestKnownScrollY / 2) + "px)";
  }

  window.addEventListener('scroll', onScroll, false);
  requestTick();

})();

/*!
 * Flickity PACKAGED v1.2.1
 * Touch, responsive, flickable galleries
 *
 * Licensed GPLv3 for open source use
 * or Flickity Commercial License for commercial use
 *
 * http://flickity.metafizzy.co
 * Copyright 2015 Metafizzy
 */

/**
 * Bridget makes jQuery widgets
 * v1.1.0
 * MIT license
 */

( function( window ) {



// -------------------------- utils -------------------------- //

var slice = Array.prototype.slice;

function noop() {}

// -------------------------- definition -------------------------- //

function defineBridget( $ ) {

// bail if no jQuery
if ( !$ ) {
  return;
}

// -------------------------- addOptionMethod -------------------------- //

/**
 * adds option method -> $().plugin('option', {...})
 * @param {Function} PluginClass - constructor class
 */
function addOptionMethod( PluginClass ) {
  // don't overwrite original option method
  if ( PluginClass.prototype.option ) {
    return;
  }

  // option setter
  PluginClass.prototype.option = function( opts ) {
    // bail out if not an object
    if ( !$.isPlainObject( opts ) ){
      return;
    }
    this.options = $.extend( true, this.options, opts );
  };
}

// -------------------------- plugin bridge -------------------------- //

// helper function for logging errors
// $.error breaks jQuery chaining
var logError = typeof console === 'undefined' ? noop :
  function( message ) {
    console.error( message );
  };

/**
 * jQuery plugin bridge, access methods like $elem.plugin('method')
 * @param {String} namespace - plugin name
 * @param {Function} PluginClass - constructor class
 */
function bridge( namespace, PluginClass ) {
  // add to jQuery fn namespace
  $.fn[ namespace ] = function( options ) {
    if ( typeof options === 'string' ) {
      // call plugin method when first argument is a string
      // get arguments for method
      var args = slice.call( arguments, 1 );

      for ( var i=0, len = this.length; i < len; i++ ) {
        var elem = this[i];
        var instance = $.data( elem, namespace );
        if ( !instance ) {
          logError( "cannot call methods on " + namespace + " prior to initialization; " +
            "attempted to call '" + options + "'" );
          continue;
        }
        if ( !$.isFunction( instance[options] ) || options.charAt(0) === '_' ) {
          logError( "no such method '" + options + "' for " + namespace + " instance" );
          continue;
        }

        // trigger method with arguments
        var returnValue = instance[ options ].apply( instance, args );

        // break look and return first value if provided
        if ( returnValue !== undefined ) {
          return returnValue;
        }
      }
      // return this if no return value
      return this;
    } else {
      return this.each( function() {
        var instance = $.data( this, namespace );
        if ( instance ) {
          // apply options & init
          instance.option( options );
          instance._init();
        } else {
          // initialize new instance
          instance = new PluginClass( this, options );
          $.data( this, namespace, instance );
        }
      });
    }
  };

}

// -------------------------- bridget -------------------------- //

/**
 * converts a Prototypical class into a proper jQuery plugin
 *   the class must have a ._init method
 * @param {String} namespace - plugin name, used in $().pluginName
 * @param {Function} PluginClass - constructor class
 */
$.bridget = function( namespace, PluginClass ) {
  addOptionMethod( PluginClass );
  bridge( namespace, PluginClass );
};

return $.bridget;

}

// transport
if ( typeof define === 'function' && define.amd ) {
  // AMD
  define( 'jquery-bridget/jquery.bridget',[ 'jquery' ], defineBridget );
} else if ( typeof exports === 'object' ) {
  defineBridget( require('jquery') );
} else {
  // get jquery from browser global
  defineBridget( window.jQuery );
}

})( window );

/*!
 * classie v1.0.1
 * class helper functions
 * from bonzo https://github.com/ded/bonzo
 * MIT license
 * 
 * classie.has( elem, 'my-class' ) -> true/false
 * classie.add( elem, 'my-new-class' )
 * classie.remove( elem, 'my-unwanted-class' )
 * classie.toggle( elem, 'my-class' )
 */

/*jshint browser: true, strict: true, undef: true, unused: true */
/*global define: false, module: false */

( function( window ) {



// class helper functions from bonzo https://github.com/ded/bonzo

function classReg( className ) {
  return new RegExp("(^|\\s+)" + className + "(\\s+|$)");
}

// classList support for class management
// altho to be fair, the api sucks because it won't accept multiple classes at once
var hasClass, addClass, removeClass;

if ( 'classList' in document.documentElement ) {
  hasClass = function( elem, c ) {
    return elem.classList.contains( c );
  };
  addClass = function( elem, c ) {
    elem.classList.add( c );
  };
  removeClass = function( elem, c ) {
    elem.classList.remove( c );
  };
}
else {
  hasClass = function( elem, c ) {
    return classReg( c ).test( elem.className );
  };
  addClass = function( elem, c ) {
    if ( !hasClass( elem, c ) ) {
      elem.className = elem.className + ' ' + c;
    }
  };
  removeClass = function( elem, c ) {
    elem.className = elem.className.replace( classReg( c ), ' ' );
  };
}

function toggleClass( elem, c ) {
  var fn = hasClass( elem, c ) ? removeClass : addClass;
  fn( elem, c );
}

var classie = {
  // full names
  hasClass: hasClass,
  addClass: addClass,
  removeClass: removeClass,
  toggleClass: toggleClass,
  // short names
  has: hasClass,
  add: addClass,
  remove: removeClass,
  toggle: toggleClass
};

// transport
if ( typeof define === 'function' && define.amd ) {
  // AMD
  define( 'classie/classie',classie );
} else if ( typeof exports === 'object' ) {
  // CommonJS
  module.exports = classie;
} else {
  // browser global
  window.classie = classie;
}

})( window );

/*!
 * EventEmitter v4.2.11 - git.io/ee
 * Unlicense - http://unlicense.org/
 * Oliver Caldwell - http://oli.me.uk/
 * @preserve
 */

;(function () {
    'use strict';

    /**
     * Class for managing events.
     * Can be extended to provide event functionality in other classes.
     *
     * @class EventEmitter Manages event registering and emitting.
     */
    function EventEmitter() {}

    // Shortcuts to improve speed and size
    var proto = EventEmitter.prototype;
    var exports = this;
    var originalGlobalValue = exports.EventEmitter;

    /**
     * Finds the index of the listener for the event in its storage array.
     *
     * @param {Function[]} listeners Array of listeners to search through.
     * @param {Function} listener Method to look for.
     * @return {Number} Index of the specified listener, -1 if not found
     * @api private
     */
    function indexOfListener(listeners, listener) {
        var i = listeners.length;
        while (i--) {
            if (listeners[i].listener === listener) {
                return i;
            }
        }

        return -1;
    }

    /**
     * Alias a method while keeping the context correct, to allow for overwriting of target method.
     *
     * @param {String} name The name of the target method.
     * @return {Function} The aliased method
     * @api private
     */
    function alias(name) {
        return function aliasClosure() {
            return this[name].apply(this, arguments);
        };
    }

    /**
     * Returns the listener array for the specified event.
     * Will initialise the event object and listener arrays if required.
     * Will return an object if you use a regex search. The object contains keys for each matched event. So /ba[rz]/ might return an object containing bar and baz. But only if you have either defined them with defineEvent or added some listeners to them.
     * Each property in the object response is an array of listener functions.
     *
     * @param {String|RegExp} evt Name of the event to return the listeners from.
     * @return {Function[]|Object} All listener functions for the event.
     */
    proto.getListeners = function getListeners(evt) {
        var events = this._getEvents();
        var response;
        var key;

        // Return a concatenated array of all matching events if
        // the selector is a regular expression.
        if (evt instanceof RegExp) {
            response = {};
            for (key in events) {
                if (events.hasOwnProperty(key) && evt.test(key)) {
                    response[key] = events[key];
                }
            }
        }
        else {
            response = events[evt] || (events[evt] = []);
        }

        return response;
    };

    /**
     * Takes a list of listener objects and flattens it into a list of listener functions.
     *
     * @param {Object[]} listeners Raw listener objects.
     * @return {Function[]} Just the listener functions.
     */
    proto.flattenListeners = function flattenListeners(listeners) {
        var flatListeners = [];
        var i;

        for (i = 0; i < listeners.length; i += 1) {
            flatListeners.push(listeners[i].listener);
        }

        return flatListeners;
    };

    /**
     * Fetches the requested listeners via getListeners but will always return the results inside an object. This is mainly for internal use but others may find it useful.
     *
     * @param {String|RegExp} evt Name of the event to return the listeners from.
     * @return {Object} All listener functions for an event in an object.
     */
    proto.getListenersAsObject = function getListenersAsObject(evt) {
        var listeners = this.getListeners(evt);
        var response;

        if (listeners instanceof Array) {
            response = {};
            response[evt] = listeners;
        }

        return response || listeners;
    };

    /**
     * Adds a listener function to the specified event.
     * The listener will not be added if it is a duplicate.
     * If the listener returns true then it will be removed after it is called.
     * If you pass a regular expression as the event name then the listener will be added to all events that match it.
     *
     * @param {String|RegExp} evt Name of the event to attach the listener to.
     * @param {Function} listener Method to be called when the event is emitted. If the function returns true then it will be removed after calling.
     * @return {Object} Current instance of EventEmitter for chaining.
     */
    proto.addListener = function addListener(evt, listener) {
        var listeners = this.getListenersAsObject(evt);
        var listenerIsWrapped = typeof listener === 'object';
        var key;

        for (key in listeners) {
            if (listeners.hasOwnProperty(key) && indexOfListener(listeners[key], listener) === -1) {
                listeners[key].push(listenerIsWrapped ? listener : {
                    listener: listener,
                    once: false
                });
            }
        }

        return this;
    };

    /**
     * Alias of addListener
     */
    proto.on = alias('addListener');

    /**
     * Semi-alias of addListener. It will add a listener that will be
     * automatically removed after its first execution.
     *
     * @param {String|RegExp} evt Name of the event to attach the listener to.
     * @param {Function} listener Method to be called when the event is emitted. If the function returns true then it will be removed after calling.
     * @return {Object} Current instance of EventEmitter for chaining.
     */
    proto.addOnceListener = function addOnceListener(evt, listener) {
        return this.addListener(evt, {
            listener: listener,
            once: true
        });
    };

    /**
     * Alias of addOnceListener.
     */
    proto.once = alias('addOnceListener');

    /**
     * Defines an event name. This is required if you want to use a regex to add a listener to multiple events at once. If you don't do this then how do you expect it to know what event to add to? Should it just add to every possible match for a regex? No. That is scary and bad.
     * You need to tell it what event names should be matched by a regex.
     *
     * @param {String} evt Name of the event to create.
     * @return {Object} Current instance of EventEmitter for chaining.
     */
    proto.defineEvent = function defineEvent(evt) {
        this.getListeners(evt);
        return this;
    };

    /**
     * Uses defineEvent to define multiple events.
     *
     * @param {String[]} evts An array of event names to define.
     * @return {Object} Current instance of EventEmitter for chaining.
     */
    proto.defineEvents = function defineEvents(evts) {
        for (var i = 0; i < evts.length; i += 1) {
            this.defineEvent(evts[i]);
        }
        return this;
    };

    /**
     * Removes a listener function from the specified event.
     * When passed a regular expression as the event name, it will remove the listener from all events that match it.
     *
     * @param {String|RegExp} evt Name of the event to remove the listener from.
     * @param {Function} listener Method to remove from the event.
     * @return {Object} Current instance of EventEmitter for chaining.
     */
    proto.removeListener = function removeListener(evt, listener) {
        var listeners = this.getListenersAsObject(evt);
        var index;
        var key;

        for (key in listeners) {
            if (listeners.hasOwnProperty(key)) {
                index = indexOfListener(listeners[key], listener);

                if (index !== -1) {
                    listeners[key].splice(index, 1);
                }
            }
        }

        return this;
    };

    /**
     * Alias of removeListener
     */
    proto.off = alias('removeListener');

    /**
     * Adds listeners in bulk using the manipulateListeners method.
     * If you pass an object as the second argument you can add to multiple events at once. The object should contain key value pairs of events and listeners or listener arrays. You can also pass it an event name and an array of listeners to be added.
     * You can also pass it a regular expression to add the array of listeners to all events that match it.
     * Yeah, this function does quite a bit. That's probably a bad thing.
     *
     * @param {String|Object|RegExp} evt An event name if you will pass an array of listeners next. An object if you wish to add to multiple events at once.
     * @param {Function[]} [listeners] An optional array of listener functions to add.
     * @return {Object} Current instance of EventEmitter for chaining.
     */
    proto.addListeners = function addListeners(evt, listeners) {
        // Pass through to manipulateListeners
        return this.manipulateListeners(false, evt, listeners);
    };

    /**
     * Removes listeners in bulk using the manipulateListeners method.
     * If you pass an object as the second argument you can remove from multiple events at once. The object should contain key value pairs of events and listeners or listener arrays.
     * You can also pass it an event name and an array of listeners to be removed.
     * You can also pass it a regular expression to remove the listeners from all events that match it.
     *
     * @param {String|Object|RegExp} evt An event name if you will pass an array of listeners next. An object if you wish to remove from multiple events at once.
     * @param {Function[]} [listeners] An optional array of listener functions to remove.
     * @return {Object} Current instance of EventEmitter for chaining.
     */
    proto.removeListeners = function removeListeners(evt, listeners) {
        // Pass through to manipulateListeners
        return this.manipulateListeners(true, evt, listeners);
    };

    /**
     * Edits listeners in bulk. The addListeners and removeListeners methods both use this to do their job. You should really use those instead, this is a little lower level.
     * The first argument will determine if the listeners are removed (true) or added (false).
     * If you pass an object as the second argument you can add/remove from multiple events at once. The object should contain key value pairs of events and listeners or listener arrays.
     * You can also pass it an event name and an array of listeners to be added/removed.
     * You can also pass it a regular expression to manipulate the listeners of all events that match it.
     *
     * @param {Boolean} remove True if you want to remove listeners, false if you want to add.
     * @param {String|Object|RegExp} evt An event name if you will pass an array of listeners next. An object if you wish to add/remove from multiple events at once.
     * @param {Function[]} [listeners] An optional array of listener functions to add/remove.
     * @return {Object} Current instance of EventEmitter for chaining.
     */
    proto.manipulateListeners = function manipulateListeners(remove, evt, listeners) {
        var i;
        var value;
        var single = remove ? this.removeListener : this.addListener;
        var multiple = remove ? this.removeListeners : this.addListeners;

        // If evt is an object then pass each of its properties to this method
        if (typeof evt === 'object' && !(evt instanceof RegExp)) {
            for (i in evt) {
                if (evt.hasOwnProperty(i) && (value = evt[i])) {
                    // Pass the single listener straight through to the singular method
                    if (typeof value === 'function') {
                        single.call(this, i, value);
                    }
                    else {
                        // Otherwise pass back to the multiple function
                        multiple.call(this, i, value);
                    }
                }
            }
        }
        else {
            // So evt must be a string
            // And listeners must be an array of listeners
            // Loop over it and pass each one to the multiple method
            i = listeners.length;
            while (i--) {
                single.call(this, evt, listeners[i]);
            }
        }

        return this;
    };

    /**
     * Removes all listeners from a specified event.
     * If you do not specify an event then all listeners will be removed.
     * That means every event will be emptied.
     * You can also pass a regex to remove all events that match it.
     *
     * @param {String|RegExp} [evt] Optional name of the event to remove all listeners for. Will remove from every event if not passed.
     * @return {Object} Current instance of EventEmitter for chaining.
     */
    proto.removeEvent = function removeEvent(evt) {
        var type = typeof evt;
        var events = this._getEvents();
        var key;

        // Remove different things depending on the state of evt
        if (type === 'string') {
            // Remove all listeners for the specified event
            delete events[evt];
        }
        else if (evt instanceof RegExp) {
            // Remove all events matching the regex.
            for (key in events) {
                if (events.hasOwnProperty(key) && evt.test(key)) {
                    delete events[key];
                }
            }
        }
        else {
            // Remove all listeners in all events
            delete this._events;
        }

        return this;
    };

    /**
     * Alias of removeEvent.
     *
     * Added to mirror the node API.
     */
    proto.removeAllListeners = alias('removeEvent');

    /**
     * Emits an event of your choice.
     * When emitted, every listener attached to that event will be executed.
     * If you pass the optional argument array then those arguments will be passed to every listener upon execution.
     * Because it uses `apply`, your array of arguments will be passed as if you wrote them out separately.
     * So they will not arrive within the array on the other side, they will be separate.
     * You can also pass a regular expression to emit to all events that match it.
     *
     * @param {String|RegExp} evt Name of the event to emit and execute listeners for.
     * @param {Array} [args] Optional array of arguments to be passed to each listener.
     * @return {Object} Current instance of EventEmitter for chaining.
     */
    proto.emitEvent = function emitEvent(evt, args) {
        var listeners = this.getListenersAsObject(evt);
        var listener;
        var i;
        var key;
        var response;

        for (key in listeners) {
            if (listeners.hasOwnProperty(key)) {
                i = listeners[key].length;

                while (i--) {
                    // If the listener returns true then it shall be removed from the event
                    // The function is executed either with a basic call or an apply if there is an args array
                    listener = listeners[key][i];

                    if (listener.once === true) {
                        this.removeListener(evt, listener.listener);
                    }

                    response = listener.listener.apply(this, args || []);

                    if (response === this._getOnceReturnValue()) {
                        this.removeListener(evt, listener.listener);
                    }
                }
            }
        }

        return this;
    };

    /**
     * Alias of emitEvent
     */
    proto.trigger = alias('emitEvent');

    /**
     * Subtly different from emitEvent in that it will pass its arguments on to the listeners, as opposed to taking a single array of arguments to pass on.
     * As with emitEvent, you can pass a regex in place of the event name to emit to all events that match it.
     *
     * @param {String|RegExp} evt Name of the event to emit and execute listeners for.
     * @param {...*} Optional additional arguments to be passed to each listener.
     * @return {Object} Current instance of EventEmitter for chaining.
     */
    proto.emit = function emit(evt) {
        var args = Array.prototype.slice.call(arguments, 1);
        return this.emitEvent(evt, args);
    };

    /**
     * Sets the current value to check against when executing listeners. If a
     * listeners return value matches the one set here then it will be removed
     * after execution. This value defaults to true.
     *
     * @param {*} value The new value to check for when executing listeners.
     * @return {Object} Current instance of EventEmitter for chaining.
     */
    proto.setOnceReturnValue = function setOnceReturnValue(value) {
        this._onceReturnValue = value;
        return this;
    };

    /**
     * Fetches the current value to check against when executing listeners. If
     * the listeners return value matches this one then it should be removed
     * automatically. It will return true by default.
     *
     * @return {*|Boolean} The current value to check for or the default, true.
     * @api private
     */
    proto._getOnceReturnValue = function _getOnceReturnValue() {
        if (this.hasOwnProperty('_onceReturnValue')) {
            return this._onceReturnValue;
        }
        else {
            return true;
        }
    };

    /**
     * Fetches the events object and creates one if required.
     *
     * @return {Object} The events storage object.
     * @api private
     */
    proto._getEvents = function _getEvents() {
        return this._events || (this._events = {});
    };

    /**
     * Reverts the global {@link EventEmitter} to its previous value and returns a reference to this version.
     *
     * @return {Function} Non conflicting EventEmitter class.
     */
    EventEmitter.noConflict = function noConflict() {
        exports.EventEmitter = originalGlobalValue;
        return EventEmitter;
    };

    // Expose the class either via AMD, CommonJS or the global object
    if (typeof define === 'function' && define.amd) {
        define('eventEmitter/EventEmitter',[],function () {
            return EventEmitter;
        });
    }
    else if (typeof module === 'object' && module.exports){
        module.exports = EventEmitter;
    }
    else {
        exports.EventEmitter = EventEmitter;
    }
}.call(this));

/*!
 * eventie v1.0.6
 * event binding helper
 *   eventie.bind( elem, 'click', myFn )
 *   eventie.unbind( elem, 'click', myFn )
 * MIT license
 */

/*jshint browser: true, undef: true, unused: true */
/*global define: false, module: false */

( function( window ) {



var docElem = document.documentElement;

var bind = function() {};

function getIEEvent( obj ) {
  var event = window.event;
  // add event.target
  event.target = event.target || event.srcElement || obj;
  return event;
}

if ( docElem.addEventListener ) {
  bind = function( obj, type, fn ) {
    obj.addEventListener( type, fn, false );
  };
} else if ( docElem.attachEvent ) {
  bind = function( obj, type, fn ) {
    obj[ type + fn ] = fn.handleEvent ?
      function() {
        var event = getIEEvent( obj );
        fn.handleEvent.call( fn, event );
      } :
      function() {
        var event = getIEEvent( obj );
        fn.call( obj, event );
      };
    obj.attachEvent( "on" + type, obj[ type + fn ] );
  };
}

var unbind = function() {};

if ( docElem.removeEventListener ) {
  unbind = function( obj, type, fn ) {
    obj.removeEventListener( type, fn, false );
  };
} else if ( docElem.detachEvent ) {
  unbind = function( obj, type, fn ) {
    obj.detachEvent( "on" + type, obj[ type + fn ] );
    try {
      delete obj[ type + fn ];
    } catch ( err ) {
      // can't delete window object properties
      obj[ type + fn ] = undefined;
    }
  };
}

var eventie = {
  bind: bind,
  unbind: unbind
};

// ----- module definition ----- //

if ( typeof define === 'function' && define.amd ) {
  // AMD
  define( 'eventie/eventie',eventie );
} else if ( typeof exports === 'object' ) {
  // CommonJS
  module.exports = eventie;
} else {
  // browser global
  window.eventie = eventie;
}

})( window );

/*!
 * getStyleProperty v1.0.4
 * original by kangax
 * http://perfectionkills.com/feature-testing-css-properties/
 * MIT license
 */

/*jshint browser: true, strict: true, undef: true */
/*global define: false, exports: false, module: false */

( function( window ) {



var prefixes = 'Webkit Moz ms Ms O'.split(' ');
var docElemStyle = document.documentElement.style;

function getStyleProperty( propName ) {
  if ( !propName ) {
    return;
  }

  // test standard property first
  if ( typeof docElemStyle[ propName ] === 'string' ) {
    return propName;
  }

  // capitalize
  propName = propName.charAt(0).toUpperCase() + propName.slice(1);

  // test vendor specific properties
  var prefixed;
  for ( var i=0, len = prefixes.length; i < len; i++ ) {
    prefixed = prefixes[i] + propName;
    if ( typeof docElemStyle[ prefixed ] === 'string' ) {
      return prefixed;
    }
  }
}

// transport
if ( typeof define === 'function' && define.amd ) {
  // AMD
  define( 'get-style-property/get-style-property',[],function() {
    return getStyleProperty;
  });
} else if ( typeof exports === 'object' ) {
  // CommonJS for Component
  module.exports = getStyleProperty;
} else {
  // browser global
  window.getStyleProperty = getStyleProperty;
}

})( window );

/*!
 * getSize v1.2.2
 * measure size of elements
 * MIT license
 */

/*jshint browser: true, strict: true, undef: true, unused: true */
/*global define: false, exports: false, require: false, module: false, console: false */

( function( window, undefined ) {



// -------------------------- helpers -------------------------- //

// get a number from a string, not a percentage
function getStyleSize( value ) {
  var num = parseFloat( value );
  // not a percent like '100%', and a number
  var isValid = value.indexOf('%') === -1 && !isNaN( num );
  return isValid && num;
}

function noop() {}

var logError = typeof console === 'undefined' ? noop :
  function( message ) {
    console.error( message );
  };

// -------------------------- measurements -------------------------- //

var measurements = [
  'paddingLeft',
  'paddingRight',
  'paddingTop',
  'paddingBottom',
  'marginLeft',
  'marginRight',
  'marginTop',
  'marginBottom',
  'borderLeftWidth',
  'borderRightWidth',
  'borderTopWidth',
  'borderBottomWidth'
];

function getZeroSize() {
  var size = {
    width: 0,
    height: 0,
    innerWidth: 0,
    innerHeight: 0,
    outerWidth: 0,
    outerHeight: 0
  };
  for ( var i=0, len = measurements.length; i < len; i++ ) {
    var measurement = measurements[i];
    size[ measurement ] = 0;
  }
  return size;
}



function defineGetSize( getStyleProperty ) {

// -------------------------- setup -------------------------- //

var isSetup = false;

var getStyle, boxSizingProp, isBoxSizeOuter;

/**
 * setup vars and functions
 * do it on initial getSize(), rather than on script load
 * For Firefox bug https://bugzilla.mozilla.org/show_bug.cgi?id=548397
 */
function setup() {
  // setup once
  if ( isSetup ) {
    return;
  }
  isSetup = true;

  var getComputedStyle = window.getComputedStyle;
  getStyle = ( function() {
    var getStyleFn = getComputedStyle ?
      function( elem ) {
        return getComputedStyle( elem, null );
      } :
      function( elem ) {
        return elem.currentStyle;
      };

      return function getStyle( elem ) {
        var style = getStyleFn( elem );
        if ( !style ) {
          logError( 'Style returned ' + style +
            '. Are you running this code in a hidden iframe on Firefox? ' +
            'See http://bit.ly/getsizebug1' );
        }
        return style;
      };
  })();

  // -------------------------- box sizing -------------------------- //

  boxSizingProp = getStyleProperty('boxSizing');

  /**
   * WebKit measures the outer-width on style.width on border-box elems
   * IE & Firefox measures the inner-width
   */
  if ( boxSizingProp ) {
    var div = document.createElement('div');
    div.style.width = '200px';
    div.style.padding = '1px 2px 3px 4px';
    div.style.borderStyle = 'solid';
    div.style.borderWidth = '1px 2px 3px 4px';
    div.style[ boxSizingProp ] = 'border-box';

    var body = document.body || document.documentElement;
    body.appendChild( div );
    var style = getStyle( div );

    isBoxSizeOuter = getStyleSize( style.width ) === 200;
    body.removeChild( div );
  }

}

// -------------------------- getSize -------------------------- //

function getSize( elem ) {
  setup();

  // use querySeletor if elem is string
  if ( typeof elem === 'string' ) {
    elem = document.querySelector( elem );
  }

  // do not proceed on non-objects
  if ( !elem || typeof elem !== 'object' || !elem.nodeType ) {
    return;
  }

  var style = getStyle( elem );

  // if hidden, everything is 0
  if ( style.display === 'none' ) {
    return getZeroSize();
  }

  var size = {};
  size.width = elem.offsetWidth;
  size.height = elem.offsetHeight;

  var isBorderBox = size.isBorderBox = !!( boxSizingProp &&
    style[ boxSizingProp ] && style[ boxSizingProp ] === 'border-box' );

  // get all measurements
  for ( var i=0, len = measurements.length; i < len; i++ ) {
    var measurement = measurements[i];
    var value = style[ measurement ];
    value = mungeNonPixel( elem, value );
    var num = parseFloat( value );
    // any 'auto', 'medium' value will be 0
    size[ measurement ] = !isNaN( num ) ? num : 0;
  }

  var paddingWidth = size.paddingLeft + size.paddingRight;
  var paddingHeight = size.paddingTop + size.paddingBottom;
  var marginWidth = size.marginLeft + size.marginRight;
  var marginHeight = size.marginTop + size.marginBottom;
  var borderWidth = size.borderLeftWidth + size.borderRightWidth;
  var borderHeight = size.borderTopWidth + size.borderBottomWidth;

  var isBorderBoxSizeOuter = isBorderBox && isBoxSizeOuter;

  // overwrite width and height if we can get it from style
  var styleWidth = getStyleSize( style.width );
  if ( styleWidth !== false ) {
    size.width = styleWidth +
      // add padding and border unless it's already including it
      ( isBorderBoxSizeOuter ? 0 : paddingWidth + borderWidth );
  }

  var styleHeight = getStyleSize( style.height );
  if ( styleHeight !== false ) {
    size.height = styleHeight +
      // add padding and border unless it's already including it
      ( isBorderBoxSizeOuter ? 0 : paddingHeight + borderHeight );
  }

  size.innerWidth = size.width - ( paddingWidth + borderWidth );
  size.innerHeight = size.height - ( paddingHeight + borderHeight );

  size.outerWidth = size.width + marginWidth;
  size.outerHeight = size.height + marginHeight;

  return size;
}

// IE8 returns percent values, not pixels
// taken from jQuery's curCSS
function mungeNonPixel( elem, value ) {
  // IE8 and has percent value
  if ( window.getComputedStyle || value.indexOf('%') === -1 ) {
    return value;
  }
  var style = elem.style;
  // Remember the original values
  var left = style.left;
  var rs = elem.runtimeStyle;
  var rsLeft = rs && rs.left;

  // Put in the new values to get a computed value out
  if ( rsLeft ) {
    rs.left = elem.currentStyle.left;
  }
  style.left = value;
  value = style.pixelLeft;

  // Revert the changed values
  style.left = left;
  if ( rsLeft ) {
    rs.left = rsLeft;
  }

  return value;
}

return getSize;

}

// transport
if ( typeof define === 'function' && define.amd ) {
  // AMD for RequireJS
  define( 'get-size/get-size',[ 'get-style-property/get-style-property' ], defineGetSize );
} else if ( typeof exports === 'object' ) {
  // CommonJS for Component
  module.exports = defineGetSize( require('desandro-get-style-property') );
} else {
  // browser global
  window.getSize = defineGetSize( window.getStyleProperty );
}

})( window );

/*!
 * docReady v1.0.4
 * Cross browser DOMContentLoaded event emitter
 * MIT license
 */

/*jshint browser: true, strict: true, undef: true, unused: true*/
/*global define: false, require: false, module: false */

( function( window ) {



var document = window.document;
// collection of functions to be triggered on ready
var queue = [];

function docReady( fn ) {
  // throw out non-functions
  if ( typeof fn !== 'function' ) {
    return;
  }

  if ( docReady.isReady ) {
    // ready now, hit it
    fn();
  } else {
    // queue function when ready
    queue.push( fn );
  }
}

docReady.isReady = false;

// triggered on various doc ready events
function onReady( event ) {
  // bail if already triggered or IE8 document is not ready just yet
  var isIE8NotReady = event.type === 'readystatechange' && document.readyState !== 'complete';
  if ( docReady.isReady || isIE8NotReady ) {
    return;
  }

  trigger();
}

function trigger() {
  docReady.isReady = true;
  // process queue
  for ( var i=0, len = queue.length; i < len; i++ ) {
    var fn = queue[i];
    fn();
  }
}

function defineDocReady( eventie ) {
  // trigger ready if page is ready
  if ( document.readyState === 'complete' ) {
    trigger();
  } else {
    // listen for events
    eventie.bind( document, 'DOMContentLoaded', onReady );
    eventie.bind( document, 'readystatechange', onReady );
    eventie.bind( window, 'load', onReady );
  }

  return docReady;
}

// transport
if ( typeof define === 'function' && define.amd ) {
  // AMD
  define( 'doc-ready/doc-ready',[ 'eventie/eventie' ], defineDocReady );
} else if ( typeof exports === 'object' ) {
  module.exports = defineDocReady( require('eventie') );
} else {
  // browser global
  window.docReady = defineDocReady( window.eventie );
}

})( window );

/**
 * matchesSelector v1.0.3
 * matchesSelector( element, '.selector' )
 * MIT license
 */

/*jshint browser: true, strict: true, undef: true, unused: true */
/*global define: false, module: false */

( function( ElemProto ) {

  'use strict';

  var matchesMethod = ( function() {
    // check for the standard method name first
    if ( ElemProto.matches ) {
      return 'matches';
    }
    // check un-prefixed
    if ( ElemProto.matchesSelector ) {
      return 'matchesSelector';
    }
    // check vendor prefixes
    var prefixes = [ 'webkit', 'moz', 'ms', 'o' ];

    for ( var i=0, len = prefixes.length; i < len; i++ ) {
      var prefix = prefixes[i];
      var method = prefix + 'MatchesSelector';
      if ( ElemProto[ method ] ) {
        return method;
      }
    }
  })();

  // ----- match ----- //

  function match( elem, selector ) {
    return elem[ matchesMethod ]( selector );
  }

  // ----- appendToFragment ----- //

  function checkParent( elem ) {
    // not needed if already has parent
    if ( elem.parentNode ) {
      return;
    }
    var fragment = document.createDocumentFragment();
    fragment.appendChild( elem );
  }

  // ----- query ----- //

  // fall back to using QSA
  // thx @jonathantneal https://gist.github.com/3062955
  function query( elem, selector ) {
    // append to fragment if no parent
    checkParent( elem );

    // match elem with all selected elems of parent
    var elems = elem.parentNode.querySelectorAll( selector );
    for ( var i=0, len = elems.length; i < len; i++ ) {
      // return true if match
      if ( elems[i] === elem ) {
        return true;
      }
    }
    // otherwise return false
    return false;
  }

  // ----- matchChild ----- //

  function matchChild( elem, selector ) {
    checkParent( elem );
    return match( elem, selector );
  }

  // ----- matchesSelector ----- //

  var matchesSelector;

  if ( matchesMethod ) {
    // IE9 supports matchesSelector, but doesn't work on orphaned elems
    // check for that
    var div = document.createElement('div');
    var supportsOrphans = match( div, 'div' );
    matchesSelector = supportsOrphans ? match : matchChild;
  } else {
    matchesSelector = query;
  }

  // transport
  if ( typeof define === 'function' && define.amd ) {
    // AMD
    define( 'matches-selector/matches-selector',[],function() {
      return matchesSelector;
    });
  } else if ( typeof exports === 'object' ) {
    module.exports = matchesSelector;
  }
  else {
    // browser global
    window.matchesSelector = matchesSelector;
  }

})( Element.prototype );

/**
 * Fizzy UI utils v1.0.1
 * MIT license
 */

/*jshint browser: true, undef: true, unused: true, strict: true */

( function( window, factory ) {
  /*global define: false, module: false, require: false */
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'fizzy-ui-utils/utils',[
      'doc-ready/doc-ready',
      'matches-selector/matches-selector'
    ], function( docReady, matchesSelector ) {
      return factory( window, docReady, matchesSelector );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      window,
      require('doc-ready'),
      require('desandro-matches-selector')
    );
  } else {
    // browser global
    window.fizzyUIUtils = factory(
      window,
      window.docReady,
      window.matchesSelector
    );
  }

}( window, function factory( window, docReady, matchesSelector ) {



var utils = {};

// ----- extend ----- //

// extends objects
utils.extend = function( a, b ) {
  for ( var prop in b ) {
    a[ prop ] = b[ prop ];
  }
  return a;
};

// ----- modulo ----- //

utils.modulo = function( num, div ) {
  return ( ( num % div ) + div ) % div;
};

// ----- isArray ----- //
  
var objToString = Object.prototype.toString;
utils.isArray = function( obj ) {
  return objToString.call( obj ) == '[object Array]';
};

// ----- makeArray ----- //

// turn element or nodeList into an array
utils.makeArray = function( obj ) {
  var ary = [];
  if ( utils.isArray( obj ) ) {
    // use object if already an array
    ary = obj;
  } else if ( obj && typeof obj.length == 'number' ) {
    // convert nodeList to array
    for ( var i=0, len = obj.length; i < len; i++ ) {
      ary.push( obj[i] );
    }
  } else {
    // array of single index
    ary.push( obj );
  }
  return ary;
};

// ----- indexOf ----- //

// index of helper cause IE8
utils.indexOf = Array.prototype.indexOf ? function( ary, obj ) {
    return ary.indexOf( obj );
  } : function( ary, obj ) {
    for ( var i=0, len = ary.length; i < len; i++ ) {
      if ( ary[i] === obj ) {
        return i;
      }
    }
    return -1;
  };

// ----- removeFrom ----- //

utils.removeFrom = function( ary, obj ) {
  var index = utils.indexOf( ary, obj );
  if ( index != -1 ) {
    ary.splice( index, 1 );
  }
};

// ----- isElement ----- //

// http://stackoverflow.com/a/384380/182183
utils.isElement = ( typeof HTMLElement == 'function' || typeof HTMLElement == 'object' ) ?
  function isElementDOM2( obj ) {
    return obj instanceof HTMLElement;
  } :
  function isElementQuirky( obj ) {
    return obj && typeof obj == 'object' &&
      obj.nodeType == 1 && typeof obj.nodeName == 'string';
  };

// ----- setText ----- //

utils.setText = ( function() {
  var setTextProperty;
  function setText( elem, text ) {
    // only check setTextProperty once
    setTextProperty = setTextProperty || ( document.documentElement.textContent !== undefined ? 'textContent' : 'innerText' );
    elem[ setTextProperty ] = text;
  }
  return setText;
})();

// ----- getParent ----- //

utils.getParent = function( elem, selector ) {
  while ( elem != document.body ) {
    elem = elem.parentNode;
    if ( matchesSelector( elem, selector ) ) {
      return elem;
    }
  }
};

// ----- getQueryElement ----- //

// use element as selector string
utils.getQueryElement = function( elem ) {
  if ( typeof elem == 'string' ) {
    return document.querySelector( elem );
  }
  return elem;
};

// ----- handleEvent ----- //

// enable .ontype to trigger from .addEventListener( elem, 'type' )
utils.handleEvent = function( event ) {
  var method = 'on' + event.type;
  if ( this[ method ] ) {
    this[ method ]( event );
  }
};

// ----- filterFindElements ----- //

utils.filterFindElements = function( elems, selector ) {
  // make array of elems
  elems = utils.makeArray( elems );
  var ffElems = [];

  for ( var i=0, len = elems.length; i < len; i++ ) {
    var elem = elems[i];
    // check that elem is an actual element
    if ( !utils.isElement( elem ) ) {
      continue;
    }
    // filter & find items if we have a selector
    if ( selector ) {
      // filter siblings
      if ( matchesSelector( elem, selector ) ) {
        ffElems.push( elem );
      }
      // find children
      var childElems = elem.querySelectorAll( selector );
      // concat childElems to filterFound array
      for ( var j=0, jLen = childElems.length; j < jLen; j++ ) {
        ffElems.push( childElems[j] );
      }
    } else {
      ffElems.push( elem );
    }
  }

  return ffElems;
};

// ----- debounceMethod ----- //

utils.debounceMethod = function( _class, methodName, threshold ) {
  // original method
  var method = _class.prototype[ methodName ];
  var timeoutName = methodName + 'Timeout';

  _class.prototype[ methodName ] = function() {
    var timeout = this[ timeoutName ];
    if ( timeout ) {
      clearTimeout( timeout );
    }
    var args = arguments;

    var _this = this;
    this[ timeoutName ] = setTimeout( function() {
      method.apply( _this, args );
      delete _this[ timeoutName ];
    }, threshold || 100 );
  };
};

// ----- htmlInit ----- //

// http://jamesroberts.name/blog/2010/02/22/string-functions-for-javascript-trim-to-camel-case-to-dashed-and-to-underscore/
utils.toDashed = function( str ) {
  return str.replace( /(.)([A-Z])/g, function( match, $1, $2 ) {
    return $1 + '-' + $2;
  }).toLowerCase();
};

var console = window.console;
/**
 * allow user to initialize classes via .js-namespace class
 * htmlInit( Widget, 'widgetName' )
 * options are parsed from data-namespace-option attribute
 */
utils.htmlInit = function( WidgetClass, namespace ) {
  docReady( function() {
    var dashedNamespace = utils.toDashed( namespace );
    var elems = document.querySelectorAll( '.js-' + dashedNamespace );
    var dataAttr = 'data-' + dashedNamespace + '-options';

    for ( var i=0, len = elems.length; i < len; i++ ) {
      var elem = elems[i];
      var attr = elem.getAttribute( dataAttr );
      var options;
      try {
        options = attr && JSON.parse( attr );
      } catch ( error ) {
        // log error, do not initialize
        if ( console ) {
          console.error( 'Error parsing ' + dataAttr + ' on ' +
            elem.nodeName.toLowerCase() + ( elem.id ? '#' + elem.id : '' ) + ': ' +
            error );
        }
        continue;
      }
      // initialize
      var instance = new WidgetClass( elem, options );
      // make available via $().data('layoutname')
      var jQuery = window.jQuery;
      if ( jQuery ) {
        jQuery.data( elem, namespace, instance );
      }
    }
  });
};

// -----  ----- //

return utils;

}));

( function( window, factory ) {
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'flickity/js/cell',[
      'get-size/get-size'
    ], function( getSize ) {
      return factory( window, getSize );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      window,
      require('get-size')
    );
  } else {
    // browser global
    window.Flickity = window.Flickity || {};
    window.Flickity.Cell = factory(
      window,
      window.getSize
    );
  }

}( window, function factory( window, getSize ) {



function Cell( elem, parent ) {
  this.element = elem;
  this.parent = parent;

  this.create();
}

var isIE8 = 'attachEvent' in window;

Cell.prototype.create = function() {
  this.element.style.position = 'absolute';
  // IE8 prevent child from changing focus http://stackoverflow.com/a/17525223/182183
  if ( isIE8 ) {
    this.element.setAttribute( 'unselectable', 'on' );
  }
  this.x = 0;
  this.shift = 0;
};

Cell.prototype.destroy = function() {
  // reset style
  this.element.style.position = '';
  var side = this.parent.originSide;
  this.element.style[ side ] = '';
};

Cell.prototype.getSize = function() {
  this.size = getSize( this.element );
};

Cell.prototype.setPosition = function( x ) {
  this.x = x;
  this.setDefaultTarget();
  this.renderPosition( x );
};

Cell.prototype.setDefaultTarget = function() {
  var marginProperty = this.parent.originSide == 'left' ? 'marginLeft' : 'marginRight';
  this.target = this.x + this.size[ marginProperty ] +
    this.size.width * this.parent.cellAlign;
};

Cell.prototype.renderPosition = function( x ) {
  // render position of cell with in slider
  var side = this.parent.originSide;
  this.element.style[ side ] = this.parent.getPositionValue( x );
};

/**
 * @param {Integer} factor - 0, 1, or -1
**/
Cell.prototype.wrapShift = function( shift ) {
  this.shift = shift;
  this.renderPosition( this.x + this.parent.slideableWidth * shift );
};

Cell.prototype.remove = function() {
  this.element.parentNode.removeChild( this.element );
};

return Cell;

}));

( function( window, factory ) {
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'flickity/js/animate',[
      'get-style-property/get-style-property',
      'fizzy-ui-utils/utils'
    ], function( getStyleProperty, utils ) {
      return factory( window, getStyleProperty, utils );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      window,
      require('desandro-get-style-property'),
      require('fizzy-ui-utils')
    );
  } else {
    // browser global
    window.Flickity = window.Flickity || {};
    window.Flickity.animatePrototype = factory(
      window,
      window.getStyleProperty,
      window.fizzyUIUtils
    );
  }

}( window, function factory( window, getStyleProperty, utils ) {



// -------------------------- requestAnimationFrame -------------------------- //

// https://gist.github.com/1866474

var lastTime = 0;
var prefixes = 'webkit moz ms o'.split(' ');
// get unprefixed rAF and cAF, if present
var requestAnimationFrame = window.requestAnimationFrame;
var cancelAnimationFrame = window.cancelAnimationFrame;
// loop through vendor prefixes and get prefixed rAF and cAF
var prefix;
for( var i = 0; i < prefixes.length; i++ ) {
  if ( requestAnimationFrame && cancelAnimationFrame ) {
    break;
  }
  prefix = prefixes[i];
  requestAnimationFrame = requestAnimationFrame || window[ prefix + 'RequestAnimationFrame' ];
  cancelAnimationFrame  = cancelAnimationFrame  || window[ prefix + 'CancelAnimationFrame' ] ||
                            window[ prefix + 'CancelRequestAnimationFrame' ];
}

// fallback to setTimeout and clearTimeout if either request/cancel is not supported
if ( !requestAnimationFrame || !cancelAnimationFrame )  {
  requestAnimationFrame = function( callback ) {
    var currTime = new Date().getTime();
    var timeToCall = Math.max( 0, 16 - ( currTime - lastTime ) );
    var id = window.setTimeout( function() {
      callback( currTime + timeToCall );
    }, timeToCall );
    lastTime = currTime + timeToCall;
    return id;
  };

  cancelAnimationFrame = function( id ) {
    window.clearTimeout( id );
  };
}

// -------------------------- animate -------------------------- //

var proto = {};

proto.startAnimation = function() {
  if ( this.isAnimating ) {
    return;
  }

  this.isAnimating = true;
  this.restingFrames = 0;
  this.animate();
};

proto.animate = function() {
  this.applyDragForce();
  this.applySelectedAttraction();

  var previousX = this.x;

  this.integratePhysics();
  this.positionSlider();
  this.settle( previousX );
  // animate next frame
  if ( this.isAnimating ) {
    var _this = this;
    requestAnimationFrame( function animateFrame() {
      _this.animate();
    });
  }

  /** /
  // log animation frame rate
  var now = new Date();
  if ( this.then ) {
    console.log( ~~( 1000 / (now-this.then)) + 'fps' )
  }
  this.then = now;
  /**/
};


var transformProperty = getStyleProperty('transform');
var is3d = !!getStyleProperty('perspective');

proto.positionSlider = function() {
  var x = this.x;
  // wrap position around
  if ( this.options.wrapAround && this.cells.length > 1 ) {
    x = utils.modulo( x, this.slideableWidth );
    x = x - this.slideableWidth;
    this.shiftWrapCells( x );
  }

  x = x + this.cursorPosition;

  // reverse if right-to-left and using transform
  x = this.options.rightToLeft && transformProperty ? -x : x;

  var value = this.getPositionValue( x );

  if ( transformProperty ) {
    // use 3D tranforms for hardware acceleration on iOS
    // but use 2D when settled, for better font-rendering
    this.slider.style[ transformProperty ] = is3d && this.isAnimating ?
      'translate3d(' + value + ',0,0)' : 'translateX(' + value + ')';
  } else {
    this.slider.style[ this.originSide ] = value;
  }
};

proto.positionSliderAtSelected = function() {
  if ( !this.cells.length ) {
    return;
  }
  var selectedCell = this.cells[ this.selectedIndex ];
  this.x = -selectedCell.target;
  this.positionSlider();
};

proto.getPositionValue = function( position ) {
  if ( this.options.percentPosition ) {
    // percent position, round to 2 digits, like 12.34%
    return ( Math.round( ( position / this.size.innerWidth ) * 10000 ) * 0.01 )+ '%';
  } else {
    // pixel positioning
    return Math.round( position ) + 'px';
  }
};

proto.settle = function( previousX ) {
  // keep track of frames where x hasn't moved
  if ( !this.isPointerDown && Math.round( this.x * 100 ) == Math.round( previousX * 100 ) ) {
    this.restingFrames++;
  }
  // stop animating if resting for 3 or more frames
  if ( this.restingFrames > 2 ) {
    this.isAnimating = false;
    delete this.isFreeScrolling;
    // render position with translateX when settled
    if ( is3d ) {
      this.positionSlider();
    }
    this.dispatchEvent('settle');
  }
};

proto.shiftWrapCells = function( x ) {
  // shift before cells
  var beforeGap = this.cursorPosition + x;
  this._shiftCells( this.beforeShiftCells, beforeGap, -1 );
  // shift after cells
  var afterGap = this.size.innerWidth - ( x + this.slideableWidth + this.cursorPosition );
  this._shiftCells( this.afterShiftCells, afterGap, 1 );
};

proto._shiftCells = function( cells, gap, shift ) {
  for ( var i=0, len = cells.length; i < len; i++ ) {
    var cell = cells[i];
    var cellShift = gap > 0 ? shift : 0;
    cell.wrapShift( cellShift );
    gap -= cell.size.outerWidth;
  }
};

proto._unshiftCells = function( cells ) {
  if ( !cells || !cells.length ) {
    return;
  }
  for ( var i=0, len = cells.length; i < len; i++ ) {
    cells[i].wrapShift( 0 );
  }
};

// -------------------------- physics -------------------------- //

proto.integratePhysics = function() {
  this.velocity += this.accel;
  this.x += this.velocity;
  this.velocity *= this.getFrictionFactor();
  // reset acceleration
  this.accel = 0;
};

proto.applyForce = function( force ) {
  this.accel += force;
};

proto.getFrictionFactor = function() {
  return 1 - this.options[ this.isFreeScrolling ? 'freeScrollFriction' : 'friction' ];
};


proto.getRestingPosition = function() {
  // my thanks to Steven Wittens, who simplified this math greatly
  return this.x + this.velocity / ( 1 - this.getFrictionFactor() );
};

proto.applyDragForce = function() {
  if ( !this.isPointerDown ) {
    return;
  }
  // change the position to drag position by applying force
  var dragVelocity = this.dragX - this.x;
  var dragForce = dragVelocity - this.velocity;
  this.applyForce( dragForce );
};

proto.applySelectedAttraction = function() {
  // do not attract if pointer down or no cells
  var len = this.cells.length;
  if ( this.isPointerDown || this.isFreeScrolling || !len ) {
    return;
  }
  var cell = this.cells[ this.selectedIndex ];
  var wrap = this.options.wrapAround && len > 1 ?
    this.slideableWidth * Math.floor( this.selectedIndex / len ) : 0;
  var distance = ( cell.target + wrap ) * -1 - this.x;
  var force = distance * this.options.selectedAttraction;
  this.applyForce( force );
};

return proto;

}));

/**
 * Flickity main
 */

( function( window, factory ) {
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'flickity/js/flickity',[
      'classie/classie',
      'eventEmitter/EventEmitter',
      'eventie/eventie',
      'get-size/get-size',
      'fizzy-ui-utils/utils',
      './cell',
      './animate'
    ], function( classie, EventEmitter, eventie, getSize, utils, Cell, animatePrototype ) {
      return factory( window, classie, EventEmitter, eventie, getSize, utils, Cell, animatePrototype );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      window,
      require('desandro-classie'),
      require('wolfy87-eventemitter'),
      require('eventie'),
      require('get-size'),
      require('fizzy-ui-utils'),
      require('./cell'),
      require('./animate')
    );
  } else {
    // browser global
    var _Flickity = window.Flickity;

    window.Flickity = factory(
      window,
      window.classie,
      window.EventEmitter,
      window.eventie,
      window.getSize,
      window.fizzyUIUtils,
      _Flickity.Cell,
      _Flickity.animatePrototype
    );
  }

}( window, function factory( window, classie, EventEmitter, eventie, getSize,
  utils, Cell, animatePrototype ) {



// vars
var jQuery = window.jQuery;
var getComputedStyle = window.getComputedStyle;
var console = window.console;

function moveElements( elems, toElem ) {
  elems = utils.makeArray( elems );
  while ( elems.length ) {
    toElem.appendChild( elems.shift() );
  }
}

// -------------------------- Flickity -------------------------- //

// globally unique identifiers
var GUID = 0;
// internal store of all Flickity intances
var instances = {};

function Flickity( element, options ) {
  var queryElement = utils.getQueryElement( element );
  if ( !queryElement ) {
    if ( console ) {
      console.error( 'Bad element for Flickity: ' + ( queryElement || element ) );
    }
    return;
  }
  this.element = queryElement;
  // add jQuery
  if ( jQuery ) {
    this.$element = jQuery( this.element );
  }
  // options
  this.options = utils.extend( {}, this.constructor.defaults );
  this.option( options );

  // kick things off
  this._create();
}

Flickity.defaults = {
  accessibility: true,
  cellAlign: 'center',
  // cellSelector: undefined,
  // contain: false,
  freeScrollFriction: 0.075, // friction when free-scrolling
  friction: 0.28, // friction when selecting
  // initialIndex: 0,
  percentPosition: true,
  resize: true,
  selectedAttraction: 0.025,
  setGallerySize: true
  // watchCSS: false,
  // wrapAround: false
};

// hash of methods triggered on _create()
Flickity.createMethods = [];

// inherit EventEmitter
utils.extend( Flickity.prototype, EventEmitter.prototype );

Flickity.prototype._create = function() {
  // add id for Flickity.data
  var id = this.guid = ++GUID;
  this.element.flickityGUID = id; // expando
  instances[ id ] = this; // associate via id
  // initial properties
  this.selectedIndex = 0;
  // how many frames slider has been in same position
  this.restingFrames = 0;
  // initial physics properties
  this.x = 0;
  this.velocity = 0;
  this.accel = 0;
  this.originSide = this.options.rightToLeft ? 'right' : 'left';
  // create viewport & slider
  this.viewport = document.createElement('div');
  this.viewport.className = 'flickity-viewport';
  Flickity.setUnselectable( this.viewport );
  this._createSlider();

  if ( this.options.resize || this.options.watchCSS ) {
    eventie.bind( window, 'resize', this );
    this.isResizeBound = true;
  }

  for ( var i=0, len = Flickity.createMethods.length; i < len; i++ ) {
    var method = Flickity.createMethods[i];
    this[ method ]();
  }

  if ( this.options.watchCSS ) {
    this.watchCSS();
  } else {
    this.activate();
  }

};

/**
 * set options
 * @param {Object} opts
 */
Flickity.prototype.option = function( opts ) {
  utils.extend( this.options, opts );
};

Flickity.prototype.activate = function() {
  if ( this.isActive ) {
    return;
  }
  this.isActive = true;
  classie.add( this.element, 'flickity-enabled' );
  if ( this.options.rightToLeft ) {
    classie.add( this.element, 'flickity-rtl' );
  }

  this.getSize();
  // move initial cell elements so they can be loaded as cells
  var cellElems = this._filterFindCellElements( this.element.children );
  moveElements( cellElems, this.slider );
  this.viewport.appendChild( this.slider );
  this.element.appendChild( this.viewport );
  // get cells from children
  this.reloadCells();

  if ( this.options.accessibility ) {
    // allow element to focusable
    this.element.tabIndex = 0;
    // listen for key presses
    eventie.bind( this.element, 'keydown', this );
  }

  this.emit('activate');

  var index;
  var initialIndex = this.options.initialIndex;
  if ( this.isInitActivated ) {
    index = this.selectedIndex;
  } else if ( initialIndex !== undefined ) {
    index = this.cells[ initialIndex ] ? initialIndex : 0;
  } else {
    index = 0;
  }
  // select instantly
  this.select( index, false, true );
  // flag for initial activation, for using initialIndex
  this.isInitActivated = true;
};

// slider positions the cells
Flickity.prototype._createSlider = function() {
  // slider element does all the positioning
  var slider = document.createElement('div');
  slider.className = 'flickity-slider';
  slider.style[ this.originSide ] = 0;
  this.slider = slider;
};

Flickity.prototype._filterFindCellElements = function( elems ) {
  return utils.filterFindElements( elems, this.options.cellSelector );
};

// goes through all children
Flickity.prototype.reloadCells = function() {
  // collection of item elements
  this.cells = this._makeCells( this.slider.children );
  this.positionCells();
  this._getWrapShiftCells();
  this.setGallerySize();
};

/**
 * turn elements into Flickity.Cells
 * @param {Array or NodeList or HTMLElement} elems
 * @returns {Array} items - collection of new Flickity Cells
 */
Flickity.prototype._makeCells = function( elems ) {
  var cellElems = this._filterFindCellElements( elems );

  // create new Flickity for collection
  var cells = [];
  for ( var i=0, len = cellElems.length; i < len; i++ ) {
    var elem = cellElems[i];
    var cell = new Cell( elem, this );
    cells.push( cell );
  }

  return cells;
};

Flickity.prototype.getLastCell = function() {
  return this.cells[ this.cells.length - 1 ];
};

// positions all cells
Flickity.prototype.positionCells = function() {
  // size all cells
  this._sizeCells( this.cells );
  // position all cells
  this._positionCells( 0 );
};

/**
 * position certain cells
 * @param {Integer} index - which cell to start with
 */
Flickity.prototype._positionCells = function( index ) {
  index = index || 0;
  // also measure maxCellHeight
  // start 0 if positioning all cells
  this.maxCellHeight = index ? this.maxCellHeight || 0 : 0;
  var cellX = 0;
  // get cellX
  if ( index > 0 ) {
    var startCell = this.cells[ index - 1 ];
    cellX = startCell.x + startCell.size.outerWidth;
  }
  var cell;
  for ( var len = this.cells.length, i=index; i < len; i++ ) {
    cell = this.cells[i];
    cell.setPosition( cellX );
    cellX += cell.size.outerWidth;
    this.maxCellHeight = Math.max( cell.size.outerHeight, this.maxCellHeight );
  }
  // keep track of cellX for wrap-around
  this.slideableWidth = cellX;
  // contain cell target
  this._containCells();
};

/**
 * cell.getSize() on multiple cells
 * @param {Array} cells
 */
Flickity.prototype._sizeCells = function( cells ) {
  for ( var i=0, len = cells.length; i < len; i++ ) {
    var cell = cells[i];
    cell.getSize();
  }
};

// alias _init for jQuery plugin .flickity()
Flickity.prototype._init =
Flickity.prototype.reposition = function() {
  this.positionCells();
  this.positionSliderAtSelected();
};

Flickity.prototype.getSize = function() {
  this.size = getSize( this.element );
  this.setCellAlign();
  this.cursorPosition = this.size.innerWidth * this.cellAlign;
};

var cellAlignShorthands = {
  // cell align, then based on origin side
  center: {
    left: 0.5,
    right: 0.5
  },
  left: {
    left: 0,
    right: 1
  },
  right: {
    right: 0,
    left: 1
  }
};

Flickity.prototype.setCellAlign = function() {
  var shorthand = cellAlignShorthands[ this.options.cellAlign ];
  this.cellAlign = shorthand ? shorthand[ this.originSide ] : this.options.cellAlign;
};

Flickity.prototype.setGallerySize = function() {
  if ( this.options.setGallerySize ) {
    this.viewport.style.height = this.maxCellHeight + 'px';
  }
};

Flickity.prototype._getWrapShiftCells = function() {
  // only for wrap-around
  if ( !this.options.wrapAround ) {
    return;
  }
  // unshift previous cells
  this._unshiftCells( this.beforeShiftCells );
  this._unshiftCells( this.afterShiftCells );
  // get before cells
  // initial gap
  var gapX = this.cursorPosition;
  var cellIndex = this.cells.length - 1;
  this.beforeShiftCells = this._getGapCells( gapX, cellIndex, -1 );
  // get after cells
  // ending gap between last cell and end of gallery viewport
  gapX = this.size.innerWidth - this.cursorPosition;
  // start cloning at first cell, working forwards
  this.afterShiftCells = this._getGapCells( gapX, 0, 1 );
};

Flickity.prototype._getGapCells = function( gapX, cellIndex, increment ) {
  // keep adding cells until the cover the initial gap
  var cells = [];
  while ( gapX > 0 ) {
    var cell = this.cells[ cellIndex ];
    if ( !cell ) {
      break;
    }
    cells.push( cell );
    cellIndex += increment;
    gapX -= cell.size.outerWidth;
  }
  return cells;
};

// ----- contain ----- //

// contain cell targets so no excess sliding
Flickity.prototype._containCells = function() {
  if ( !this.options.contain || this.options.wrapAround || !this.cells.length ) {
    return;
  }
  var startMargin = this.options.rightToLeft ? 'marginRight' : 'marginLeft';
  var endMargin = this.options.rightToLeft ? 'marginLeft' : 'marginRight';
  var firstCellStartMargin = this.cells[0].size[ startMargin ];
  var lastCell = this.getLastCell();
  var contentWidth = this.slideableWidth - lastCell.size[ endMargin ];
  var endLimit = contentWidth - this.size.innerWidth * ( 1 - this.cellAlign );
  // content is less than gallery size
  var isContentSmaller = contentWidth < this.size.innerWidth;
  // contain each cell target
  for ( var i=0, len = this.cells.length; i < len; i++ ) {
    var cell = this.cells[i];
    // reset default target
    cell.setDefaultTarget();
    if ( isContentSmaller ) {
      // all cells fit inside gallery
      cell.target = contentWidth * this.cellAlign;
    } else {
      // contain to bounds
      cell.target = Math.max( cell.target, this.cursorPosition + firstCellStartMargin );
      cell.target = Math.min( cell.target, endLimit );
    }
  }
};

// -----  ----- //

/**
 * emits events via eventEmitter and jQuery events
 * @param {String} type - name of event
 * @param {Event} event - original event
 * @param {Array} args - extra arguments
 */
Flickity.prototype.dispatchEvent = function( type, event, args ) {
  var emitArgs = [ event ].concat( args );
  this.emitEvent( type, emitArgs );

  if ( jQuery && this.$element ) {
    if ( event ) {
      // create jQuery event
      var $event = jQuery.Event( event );
      $event.type = type;
      this.$element.trigger( $event, args );
    } else {
      // just trigger with type if no event available
      this.$element.trigger( type, args );
    }
  }
};

// -------------------------- select -------------------------- //

/**
 * @param {Integer} index - index of the cell
 * @param {Boolean} isWrap - will wrap-around to last/first if at the end
 * @param {Boolean} isInstant - will immediately set position at selected cell
 */
Flickity.prototype.select = function( index, isWrap, isInstant ) {
  if ( !this.isActive ) {
    return;
  }
  index = parseInt( index, 10 );
  // wrap position so slider is within normal area
  var len = this.cells.length;
  if ( this.options.wrapAround && len > 1 ) {
    if ( index < 0 ) {
      this.x -= this.slideableWidth;
    } else if ( index >= len ) {
      this.x += this.slideableWidth;
    }
  }

  if ( this.options.wrapAround || isWrap ) {
    index = utils.modulo( index, len );
  }
  // bail if invalid index
  if ( !this.cells[ index ] ) {
    return;
  }
  this.selectedIndex = index;
  this.setSelectedCell();
  if ( isInstant ) {
    this.positionSliderAtSelected();
  } else {
    this.startAnimation();
  }
  this.dispatchEvent('cellSelect');
};

Flickity.prototype.previous = function( isWrap ) {
  this.select( this.selectedIndex - 1, isWrap );
};

Flickity.prototype.next = function( isWrap ) {
  this.select( this.selectedIndex + 1, isWrap );
};

Flickity.prototype.setSelectedCell = function() {
  this._removeSelectedCellClass();
  this.selectedCell = this.cells[ this.selectedIndex ];
  this.selectedElement = this.selectedCell.element;
  classie.add( this.selectedElement, 'is-selected' );
};

Flickity.prototype._removeSelectedCellClass = function() {
  if ( this.selectedCell ) {
    classie.remove( this.selectedCell.element, 'is-selected' );
  }
};

// -------------------------- get cells -------------------------- //

/**
 * get Flickity.Cell, given an Element
 * @param {Element} elem
 * @returns {Flickity.Cell} item
 */
Flickity.prototype.getCell = function( elem ) {
  // loop through cells to get the one that matches
  for ( var i=0, len = this.cells.length; i < len; i++ ) {
    var cell = this.cells[i];
    if ( cell.element == elem ) {
      return cell;
    }
  }
};

/**
 * get collection of Flickity.Cells, given Elements
 * @param {Element, Array, NodeList} elems
 * @returns {Array} cells - Flickity.Cells
 */
Flickity.prototype.getCells = function( elems ) {
  elems = utils.makeArray( elems );
  var cells = [];
  for ( var i=0, len = elems.length; i < len; i++ ) {
    var elem = elems[i];
    var cell = this.getCell( elem );
    if ( cell ) {
      cells.push( cell );
    }
  }
  return cells;
};

/**
 * get cell elements
 * @returns {Array} cellElems
 */
Flickity.prototype.getCellElements = function() {
  var cellElems = [];
  for ( var i=0, len = this.cells.length; i < len; i++ ) {
    cellElems.push( this.cells[i].element );
  }
  return cellElems;
};

/**
 * get parent cell from an element
 * @param {Element} elem
 * @returns {Flickit.Cell} cell
 */
Flickity.prototype.getParentCell = function( elem ) {
  // first check if elem is cell
  var cell = this.getCell( elem );
  if ( cell ) {
    return cell;
  }
  // try to get parent cell elem
  elem = utils.getParent( elem, '.flickity-slider > *' );
  return this.getCell( elem );
};

/**
 * get cells adjacent to a cell
 * @param {Integer} adjCount - number of adjacent cells
 * @param {Integer} index - index of cell to start
 * @returns {Array} cells - array of Flickity.Cells
 */
Flickity.prototype.getAdjacentCellElements = function( adjCount, index ) {
  if ( !adjCount ) {
    return [ this.selectedElement ];
  }
  index = index === undefined ? this.selectedIndex : index;

  var len = this.cells.length;
  if ( 1 + ( adjCount * 2 ) >= len ) {
    return this.getCellElements();
  }

  var cellElems = [];
  for ( var i = index - adjCount; i <= index + adjCount ; i++ ) {
    var cellIndex = this.options.wrapAround ? utils.modulo( i, len ) : i;
    var cell = this.cells[ cellIndex ];
    if ( cell ) {
      cellElems.push( cell.element );
    }
  }
  return cellElems;
};

// -------------------------- events -------------------------- //

Flickity.prototype.uiChange = function() {
  this.emit('uiChange');
};

Flickity.prototype.childUIPointerDown = function( event ) {
  this.emitEvent( 'childUIPointerDown', [ event ] );
};

// ----- resize ----- //

Flickity.prototype.onresize = function() {
  this.watchCSS();
  this.resize();
};

utils.debounceMethod( Flickity, 'onresize', 150 );

Flickity.prototype.resize = function() {
  if ( !this.isActive ) {
    return;
  }
  this.getSize();
  // wrap values
  if ( this.options.wrapAround ) {
    this.x = utils.modulo( this.x, this.slideableWidth );
  }
  this.positionCells();
  this._getWrapShiftCells();
  this.setGallerySize();
  this.positionSliderAtSelected();
};

var supportsConditionalCSS = Flickity.supportsConditionalCSS = ( function() {
  var supports;
  return function checkSupport() {
    if ( supports !== undefined ) {
      return supports;
    }
    if ( !getComputedStyle ) {
      supports = false;
      return;
    }
    // style body's :after and check that
    var style = document.createElement('style');
    var cssText = document.createTextNode('body:after { content: "foo"; display: none; }');
    style.appendChild( cssText );
    document.head.appendChild( style );
    var afterContent = getComputedStyle( document.body, ':after' ).content;
    // check if able to get :after content
    supports = afterContent.indexOf('foo') != -1;
    document.head.removeChild( style );
    return supports;
  };
})();

// watches the :after property, activates/deactivates
Flickity.prototype.watchCSS = function() {
  var watchOption = this.options.watchCSS;
  if ( !watchOption ) {
    return;
  }
  var supports = supportsConditionalCSS();
  if ( !supports ) {
    // activate if watch option is fallbackOn
    var method = watchOption == 'fallbackOn' ? 'activate' : 'deactivate';
    this[ method ]();
    return;
  }

  var afterContent = getComputedStyle( this.element, ':after' ).content;
  // activate if :after { content: 'flickity' }
  if ( afterContent.indexOf('flickity') != -1 ) {
    this.activate();
  } else {
    this.deactivate();
  }
};

// ----- keydown ----- //

// go previous/next if left/right keys pressed
Flickity.prototype.onkeydown = function( event ) {
  // only work if element is in focus
  if ( !this.options.accessibility ||
    ( document.activeElement && document.activeElement != this.element ) ) {
    return;
  }

  if ( event.keyCode == 37 ) {
    // go left
    var leftMethod = this.options.rightToLeft ? 'next' : 'previous';
    this.uiChange();
    this[ leftMethod ]();
  } else if ( event.keyCode == 39 ) {
    // go right
    var rightMethod = this.options.rightToLeft ? 'previous' : 'next';
    this.uiChange();
    this[ rightMethod ]();
  }
};

// -------------------------- destroy -------------------------- //

// deactivate all Flickity functionality, but keep stuff available
Flickity.prototype.deactivate = function() {
  if ( !this.isActive ) {
    return;
  }
  classie.remove( this.element, 'flickity-enabled' );
  classie.remove( this.element, 'flickity-rtl' );
  // destroy cells
  for ( var i=0, len = this.cells.length; i < len; i++ ) {
    var cell = this.cells[i];
    cell.destroy();
  }
  this._removeSelectedCellClass();
  this.element.removeChild( this.viewport );
  // move child elements back into element
  moveElements( this.slider.children, this.element );
  if ( this.options.accessibility ) {
    this.element.removeAttribute('tabIndex');
    eventie.unbind( this.element, 'keydown', this );
  }
  // set flags
  this.isActive = false;
  this.emit('deactivate');
};

Flickity.prototype.destroy = function() {
  this.deactivate();
  if ( this.isResizeBound ) {
    eventie.unbind( window, 'resize', this );
  }
  this.emit('destroy');
  if ( jQuery && this.$element ) {
    jQuery.removeData( this.element, 'flickity' );
  }
  delete this.element.flickityGUID;
  delete instances[ this.guid ];
};

// -------------------------- prototype -------------------------- //

utils.extend( Flickity.prototype, animatePrototype );

// -------------------------- extras -------------------------- //

// quick check for IE8
var isIE8 = 'attachEvent' in window;

Flickity.setUnselectable = function( elem ) {
  if ( !isIE8 ) {
    return;
  }
  // IE8 prevent child from changing focus http://stackoverflow.com/a/17525223/182183
  elem.setAttribute( 'unselectable', 'on' );
};

/**
 * get Flickity instance from element
 * @param {Element} elem
 * @returns {Flickity}
 */
Flickity.data = function( elem ) {
  elem = utils.getQueryElement( elem );
  var id = elem && elem.flickityGUID;
  return id && instances[ id ];
};

utils.htmlInit( Flickity, 'flickity' );

if ( jQuery && jQuery.bridget ) {
  jQuery.bridget( 'flickity', Flickity );
}

Flickity.Cell = Cell;

return Flickity;

}));

/*!
 * Unipointer v1.1.0
 * base class for doing one thing with pointer event
 * MIT license
 */

/*jshint browser: true, undef: true, unused: true, strict: true */
/*global define: false, module: false, require: false */

( function( window, factory ) {
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'unipointer/unipointer',[
      'eventEmitter/EventEmitter',
      'eventie/eventie'
    ], function( EventEmitter, eventie ) {
      return factory( window, EventEmitter, eventie );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      window,
      require('wolfy87-eventemitter'),
      require('eventie')
    );
  } else {
    // browser global
    window.Unipointer = factory(
      window,
      window.EventEmitter,
      window.eventie
    );
  }

}( window, function factory( window, EventEmitter, eventie ) {



function noop() {}

function Unipointer() {}

// inherit EventEmitter
Unipointer.prototype = new EventEmitter();

Unipointer.prototype.bindStartEvent = function( elem ) {
  this._bindStartEvent( elem, true );
};

Unipointer.prototype.unbindStartEvent = function( elem ) {
  this._bindStartEvent( elem, false );
};

/**
 * works as unbinder, as you can ._bindStart( false ) to unbind
 * @param {Boolean} isBind - will unbind if falsey
 */
Unipointer.prototype._bindStartEvent = function( elem, isBind ) {
  // munge isBind, default to true
  isBind = isBind === undefined ? true : !!isBind;
  var bindMethod = isBind ? 'bind' : 'unbind';

  if ( window.navigator.pointerEnabled ) {
    // W3C Pointer Events, IE11. See https://coderwall.com/p/mfreca
    eventie[ bindMethod ]( elem, 'pointerdown', this );
  } else if ( window.navigator.msPointerEnabled ) {
    // IE10 Pointer Events
    eventie[ bindMethod ]( elem, 'MSPointerDown', this );
  } else {
    // listen for both, for devices like Chrome Pixel
    eventie[ bindMethod ]( elem, 'mousedown', this );
    eventie[ bindMethod ]( elem, 'touchstart', this );
  }
};

// trigger handler methods for events
Unipointer.prototype.handleEvent = function( event ) {
  var method = 'on' + event.type;
  if ( this[ method ] ) {
    this[ method ]( event );
  }
};

// returns the touch that we're keeping track of
Unipointer.prototype.getTouch = function( touches ) {
  for ( var i=0, len = touches.length; i < len; i++ ) {
    var touch = touches[i];
    if ( touch.identifier == this.pointerIdentifier ) {
      return touch;
    }
  }
};

// ----- start event ----- //

Unipointer.prototype.onmousedown = function( event ) {
  // dismiss clicks from right or middle buttons
  var button = event.button;
  if ( button && ( button !== 0 && button !== 1 ) ) {
    return;
  }
  this._pointerDown( event, event );
};

Unipointer.prototype.ontouchstart = function( event ) {
  this._pointerDown( event, event.changedTouches[0] );
};

Unipointer.prototype.onMSPointerDown =
Unipointer.prototype.onpointerdown = function( event ) {
  this._pointerDown( event, event );
};

/**
 * pointer start
 * @param {Event} event
 * @param {Event or Touch} pointer
 */
Unipointer.prototype._pointerDown = function( event, pointer ) {
  // dismiss other pointers
  if ( this.isPointerDown ) {
    return;
  }

  this.isPointerDown = true;
  // save pointer identifier to match up touch events
  this.pointerIdentifier = pointer.pointerId !== undefined ?
    // pointerId for pointer events, touch.indentifier for touch events
    pointer.pointerId : pointer.identifier;

  this.pointerDown( event, pointer );
};

Unipointer.prototype.pointerDown = function( event, pointer ) {
  this._bindPostStartEvents( event );
  this.emitEvent( 'pointerDown', [ event, pointer ] );
};

// hash of events to be bound after start event
var postStartEvents = {
  mousedown: [ 'mousemove', 'mouseup' ],
  touchstart: [ 'touchmove', 'touchend', 'touchcancel' ],
  pointerdown: [ 'pointermove', 'pointerup', 'pointercancel' ],
  MSPointerDown: [ 'MSPointerMove', 'MSPointerUp', 'MSPointerCancel' ]
};

Unipointer.prototype._bindPostStartEvents = function( event ) {
  if ( !event ) {
    return;
  }
  // get proper events to match start event
  var events = postStartEvents[ event.type ];
  // IE8 needs to be bound to document
  var node = event.preventDefault ? window : document;
  // bind events to node
  for ( var i=0, len = events.length; i < len; i++ ) {
    var evnt = events[i];
    eventie.bind( node, evnt, this );
  }
  // save these arguments
  this._boundPointerEvents = {
    events: events,
    node: node
  };
};

Unipointer.prototype._unbindPostStartEvents = function() {
  var args = this._boundPointerEvents;
  // IE8 can trigger dragEnd twice, check for _boundEvents
  if ( !args || !args.events ) {
    return;
  }

  for ( var i=0, len = args.events.length; i < len; i++ ) {
    var event = args.events[i];
    eventie.unbind( args.node, event, this );
  }
  delete this._boundPointerEvents;
};

// ----- move event ----- //

Unipointer.prototype.onmousemove = function( event ) {
  this._pointerMove( event, event );
};

Unipointer.prototype.onMSPointerMove =
Unipointer.prototype.onpointermove = function( event ) {
  if ( event.pointerId == this.pointerIdentifier ) {
    this._pointerMove( event, event );
  }
};

Unipointer.prototype.ontouchmove = function( event ) {
  var touch = this.getTouch( event.changedTouches );
  if ( touch ) {
    this._pointerMove( event, touch );
  }
};

/**
 * pointer move
 * @param {Event} event
 * @param {Event or Touch} pointer
 * @private
 */
Unipointer.prototype._pointerMove = function( event, pointer ) {
  this.pointerMove( event, pointer );
};

// public
Unipointer.prototype.pointerMove = function( event, pointer ) {
  this.emitEvent( 'pointerMove', [ event, pointer ] );
};

// ----- end event ----- //


Unipointer.prototype.onmouseup = function( event ) {
  this._pointerUp( event, event );
};

Unipointer.prototype.onMSPointerUp =
Unipointer.prototype.onpointerup = function( event ) {
  if ( event.pointerId == this.pointerIdentifier ) {
    this._pointerUp( event, event );
  }
};

Unipointer.prototype.ontouchend = function( event ) {
  var touch = this.getTouch( event.changedTouches );
  if ( touch ) {
    this._pointerUp( event, touch );
  }
};

/**
 * pointer up
 * @param {Event} event
 * @param {Event or Touch} pointer
 * @private
 */
Unipointer.prototype._pointerUp = function( event, pointer ) {
  this._pointerDone();
  this.pointerUp( event, pointer );
};

// public
Unipointer.prototype.pointerUp = function( event, pointer ) {
  this.emitEvent( 'pointerUp', [ event, pointer ] );
};

// ----- pointer done ----- //

// triggered on pointer up & pointer cancel
Unipointer.prototype._pointerDone = function() {
  // reset properties
  this.isPointerDown = false;
  delete this.pointerIdentifier;
  // remove events
  this._unbindPostStartEvents();
  this.pointerDone();
};

Unipointer.prototype.pointerDone = noop;

// ----- pointer cancel ----- //

Unipointer.prototype.onMSPointerCancel =
Unipointer.prototype.onpointercancel = function( event ) {
  if ( event.pointerId == this.pointerIdentifier ) {
    this._pointerCancel( event, event );
  }
};

Unipointer.prototype.ontouchcancel = function( event ) {
  var touch = this.getTouch( event.changedTouches );
  if ( touch ) {
    this._pointerCancel( event, touch );
  }
};

/**
 * pointer cancel
 * @param {Event} event
 * @param {Event or Touch} pointer
 * @private
 */
Unipointer.prototype._pointerCancel = function( event, pointer ) {
  this._pointerDone();
  this.pointerCancel( event, pointer );
};

// public
Unipointer.prototype.pointerCancel = function( event, pointer ) {
  this.emitEvent( 'pointerCancel', [ event, pointer ] );
};

// -----  ----- //

// utility function for getting x/y cooridinates from event, because IE8
Unipointer.getPointerPoint = function( pointer ) {
  return {
    x: pointer.pageX !== undefined ? pointer.pageX : pointer.clientX,
    y: pointer.pageY !== undefined ? pointer.pageY : pointer.clientY
  };
};

// -----  ----- //

return Unipointer;

}));

/*!
 * Unidragger v1.1.6
 * Draggable base class
 * MIT license
 */

/*jshint browser: true, unused: true, undef: true, strict: true */

( function( window, factory ) {
  /*global define: false, module: false, require: false */
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'unidragger/unidragger',[
      'eventie/eventie',
      'unipointer/unipointer'
    ], function( eventie, Unipointer ) {
      return factory( window, eventie, Unipointer );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      window,
      require('eventie'),
      require('unipointer')
    );
  } else {
    // browser global
    window.Unidragger = factory(
      window,
      window.eventie,
      window.Unipointer
    );
  }

}( window, function factory( window, eventie, Unipointer ) {



// -----  ----- //

function noop() {}

// handle IE8 prevent default
function preventDefaultEvent( event ) {
  if ( event.preventDefault ) {
    event.preventDefault();
  } else {
    event.returnValue = false;
  }
}

// -------------------------- Unidragger -------------------------- //

function Unidragger() {}

// inherit Unipointer & EventEmitter
Unidragger.prototype = new Unipointer();

// ----- bind start ----- //

Unidragger.prototype.bindHandles = function() {
  this._bindHandles( true );
};

Unidragger.prototype.unbindHandles = function() {
  this._bindHandles( false );
};

var navigator = window.navigator;
/**
 * works as unbinder, as you can .bindHandles( false ) to unbind
 * @param {Boolean} isBind - will unbind if falsey
 */
Unidragger.prototype._bindHandles = function( isBind ) {
  // munge isBind, default to true
  isBind = isBind === undefined ? true : !!isBind;
  // extra bind logic
  var binderExtra;
  if ( navigator.pointerEnabled ) {
    binderExtra = function( handle ) {
      // disable scrolling on the element
      handle.style.touchAction = isBind ? 'none' : '';
    };
  } else if ( navigator.msPointerEnabled ) {
    binderExtra = function( handle ) {
      // disable scrolling on the element
      handle.style.msTouchAction = isBind ? 'none' : '';
    };
  } else {
    binderExtra = function() {
      // TODO re-enable img.ondragstart when unbinding
      if ( isBind ) {
        disableImgOndragstart( handle );
      }
    };
  }
  // bind each handle
  var bindMethod = isBind ? 'bind' : 'unbind';
  for ( var i=0, len = this.handles.length; i < len; i++ ) {
    var handle = this.handles[i];
    this._bindStartEvent( handle, isBind );
    binderExtra( handle );
    eventie[ bindMethod ]( handle, 'click', this );
  }
};

// remove default dragging interaction on all images in IE8
// IE8 does its own drag thing on images, which messes stuff up

function noDragStart() {
  return false;
}

// TODO replace this with a IE8 test
var isIE8 = 'attachEvent' in document.documentElement;

// IE8 only
var disableImgOndragstart = !isIE8 ? noop : function( handle ) {

  if ( handle.nodeName == 'IMG' ) {
    handle.ondragstart = noDragStart;
  }

  var images = handle.querySelectorAll('img');
  for ( var i=0, len = images.length; i < len; i++ ) {
    var img = images[i];
    img.ondragstart = noDragStart;
  }
};

// ----- start event ----- //

/**
 * pointer start
 * @param {Event} event
 * @param {Event or Touch} pointer
 */
Unidragger.prototype.pointerDown = function( event, pointer ) {
  // dismiss range sliders
  if ( event.target.nodeName == 'INPUT' && event.target.type == 'range' ) {
    // reset pointerDown logic
    this.isPointerDown = false;
    delete this.pointerIdentifier;
    return;
  }

  this._dragPointerDown( event, pointer );
  // kludge to blur focused inputs in dragger
  var focused = document.activeElement;
  if ( focused && focused.blur ) {
    focused.blur();
  }
  // bind move and end events
  this._bindPostStartEvents( event );
  // track scrolling
  this.pointerDownScroll = Unidragger.getScrollPosition();
  eventie.bind( window, 'scroll', this );

  this.emitEvent( 'pointerDown', [ event, pointer ] );
};

// base pointer down logic
Unidragger.prototype._dragPointerDown = function( event, pointer ) {
  // track to see when dragging starts
  this.pointerDownPoint = Unipointer.getPointerPoint( pointer );

  // prevent default, unless touchstart or <select>
  var isTouchstart = event.type == 'touchstart';
  var targetNodeName = event.target.nodeName;
  if ( !isTouchstart && targetNodeName != 'SELECT' ) {
    preventDefaultEvent( event );
  }
};

// ----- move event ----- //

/**
 * drag move
 * @param {Event} event
 * @param {Event or Touch} pointer
 */
Unidragger.prototype.pointerMove = function( event, pointer ) {
  var moveVector = this._dragPointerMove( event, pointer );
  this.emitEvent( 'pointerMove', [ event, pointer, moveVector ] );
  this._dragMove( event, pointer, moveVector );
};

// base pointer move logic
Unidragger.prototype._dragPointerMove = function( event, pointer ) {
  var movePoint = Unipointer.getPointerPoint( pointer );
  var moveVector = {
    x: movePoint.x - this.pointerDownPoint.x,
    y: movePoint.y - this.pointerDownPoint.y
  };
  // start drag if pointer has moved far enough to start drag
  if ( !this.isDragging && this.hasDragStarted( moveVector ) ) {
    this._dragStart( event, pointer );
  }
  return moveVector;
};

// condition if pointer has moved far enough to start drag
Unidragger.prototype.hasDragStarted = function( moveVector ) {
  return Math.abs( moveVector.x ) > 3 || Math.abs( moveVector.y ) > 3;
};


// ----- end event ----- //

/**
 * pointer up
 * @param {Event} event
 * @param {Event or Touch} pointer
 */
Unidragger.prototype.pointerUp = function( event, pointer ) {
  this.emitEvent( 'pointerUp', [ event, pointer ] );
  this._dragPointerUp( event, pointer );
};

Unidragger.prototype._dragPointerUp = function( event, pointer ) {
  if ( this.isDragging ) {
    this._dragEnd( event, pointer );
  } else {
    // pointer didn't move enough for drag to start
    this._staticClick( event, pointer );
  }
};

Unidragger.prototype.pointerDone = function() {
  eventie.unbind( window, 'scroll', this );
};

// -------------------------- drag -------------------------- //

// dragStart
Unidragger.prototype._dragStart = function( event, pointer ) {
  this.isDragging = true;
  this.dragStartPoint = Unidragger.getPointerPoint( pointer );
  // prevent clicks
  this.isPreventingClicks = true;

  this.dragStart( event, pointer );
};

Unidragger.prototype.dragStart = function( event, pointer ) {
  this.emitEvent( 'dragStart', [ event, pointer ] );
};

// dragMove
Unidragger.prototype._dragMove = function( event, pointer, moveVector ) {
  // do not drag if not dragging yet
  if ( !this.isDragging ) {
    return;
  }

  this.dragMove( event, pointer, moveVector );
};

Unidragger.prototype.dragMove = function( event, pointer, moveVector ) {
  preventDefaultEvent( event );
  this.emitEvent( 'dragMove', [ event, pointer, moveVector ] );
};

// dragEnd
Unidragger.prototype._dragEnd = function( event, pointer ) {
  // set flags
  this.isDragging = false;
  // re-enable clicking async
  var _this = this;
  setTimeout( function() {
    delete _this.isPreventingClicks;
  });

  this.dragEnd( event, pointer );
};

Unidragger.prototype.dragEnd = function( event, pointer ) {
  this.emitEvent( 'dragEnd', [ event, pointer ] );
};

Unidragger.prototype.pointerDone = function() {
  eventie.unbind( window, 'scroll', this );
  delete this.pointerDownScroll;
};

// ----- onclick ----- //

// handle all clicks and prevent clicks when dragging
Unidragger.prototype.onclick = function( event ) {
  if ( this.isPreventingClicks ) {
    preventDefaultEvent( event );
  }
};

// ----- staticClick ----- //

// triggered after pointer down & up with no/tiny movement
Unidragger.prototype._staticClick = function( event, pointer ) {
  // ignore emulated mouse up clicks
  if ( this.isIgnoringMouseUp && event.type == 'mouseup' ) {
    return;
  }

  // allow click in <input>s and <textarea>s
  var nodeName = event.target.nodeName;
  if ( nodeName == 'INPUT' || nodeName == 'TEXTAREA' ) {
    event.target.focus();
  }
  this.staticClick( event, pointer );

  // set flag for emulated clicks 300ms after touchend
  if ( event.type != 'mouseup' ) {
    this.isIgnoringMouseUp = true;
    var _this = this;
    // reset flag after 300ms
    setTimeout( function() {
      delete _this.isIgnoringMouseUp;
    }, 400 );
  }
};

Unidragger.prototype.staticClick = function( event, pointer ) {
  this.emitEvent( 'staticClick', [ event, pointer ] );
};

// ----- scroll ----- //

Unidragger.prototype.onscroll = function() {
  var scroll = Unidragger.getScrollPosition();
  var scrollMoveX = this.pointerDownScroll.x - scroll.x;
  var scrollMoveY = this.pointerDownScroll.y - scroll.y;
  // cancel click/tap if scroll is too much
  if ( Math.abs( scrollMoveX ) > 3 || Math.abs( scrollMoveY ) > 3 ) {
    this._pointerDone();
  }
};

// ----- utils ----- //

Unidragger.getPointerPoint = function( pointer ) {
  return {
    x: pointer.pageX !== undefined ? pointer.pageX : pointer.clientX,
    y: pointer.pageY !== undefined ? pointer.pageY : pointer.clientY
  };
};

var isPageOffset = window.pageYOffset !== undefined;

// get scroll in { x, y }
Unidragger.getScrollPosition = function() {
  return {
    x: isPageOffset ? window.pageXOffset : document.body.scrollLeft,
    y: isPageOffset ? window.pageYOffset : document.body.scrollTop
  };
};

// -----  ----- //

Unidragger.getPointerPoint = Unipointer.getPointerPoint;

return Unidragger;

}));

( function( window, factory ) {
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'flickity/js/drag',[
      'classie/classie',
      'eventie/eventie',
      './flickity',
      'unidragger/unidragger',
      'fizzy-ui-utils/utils'
    ], function( classie, eventie, Flickity, Unidragger, utils ) {
      return factory( window, classie, eventie, Flickity, Unidragger, utils );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      window,
      require('desandro-classie'),
      require('eventie'),
      require('./flickity'),
      require('unidragger'),
      require('fizzy-ui-utils')
    );
  } else {
    // browser global
    window.Flickity = factory(
      window,
      window.classie,
      window.eventie,
      window.Flickity,
      window.Unidragger,
      window.fizzyUIUtils
    );
  }

}( window, function factory( window, classie, eventie, Flickity, Unidragger, utils ) {



// handle IE8 prevent default
function preventDefaultEvent( event ) {
  if ( event.preventDefault ) {
    event.preventDefault();
  } else {
    event.returnValue = false;
  }
}

// ----- defaults ----- //

utils.extend( Flickity.defaults, {
  draggable: true
});

// ----- create ----- //

Flickity.createMethods.push('_createDrag');

// -------------------------- drag prototype -------------------------- //

utils.extend( Flickity.prototype, Unidragger.prototype );

// --------------------------  -------------------------- //

Flickity.prototype._createDrag = function() {
  this.on( 'activate', this.bindDrag );
  this.on( 'uiChange', this._uiChangeDrag );
  this.on( 'childUIPointerDown', this._childUIPointerDownDrag );
  this.on( 'deactivate', this.unbindDrag );
};

Flickity.prototype.bindDrag = function() {
  if ( !this.options.draggable || this.isDragBound ) {
    return;
  }
  classie.add( this.element, 'is-draggable' );
  this.handles = [ this.viewport ];
  this.bindHandles();
  this.isDragBound = true;
};

Flickity.prototype.unbindDrag = function() {
  if ( !this.isDragBound ) {
    return;
  }
  classie.remove( this.element, 'is-draggable' );
  this.unbindHandles();
  delete this.isDragBound;
};

Flickity.prototype._uiChangeDrag = function() {
  delete this.isFreeScrolling;
};

Flickity.prototype._childUIPointerDownDrag = function( event ) {
  preventDefaultEvent( event );
  this.pointerDownFocus( event );
};

// -------------------------- pointer events -------------------------- //

Flickity.prototype.pointerDown = function( event, pointer ) {
  // dismiss range sliders
  if ( event.target.nodeName == 'INPUT' && event.target.type == 'range' ) {
    // reset pointerDown logic
    this.isPointerDown = false;
    delete this.pointerIdentifier;
    return;
  }

  this._dragPointerDown( event, pointer );

  // kludge to blur focused inputs in dragger
  var focused = document.activeElement;
  if ( focused && focused.blur && focused != this.element &&
    // do not blur body for IE9 & 10, #117
    focused != document.body ) {
    focused.blur();
  }
  this.pointerDownFocus( event );
  // stop if it was moving
  this.dragX = this.x;
  classie.add( this.viewport, 'is-pointer-down' );
  // bind move and end events
  this._bindPostStartEvents( event );
  // track scrolling
  this.pointerDownScroll = Unidragger.getScrollPosition();
  eventie.bind( window, 'scroll', this );

  this.dispatchEvent( 'pointerDown', event, [ pointer ] );
};

var touchStartEvents = {
  touchstart: true,
  MSPointerDown: true
};

var focusNodes = {
  INPUT: true,
  SELECT: true
};

Flickity.prototype.pointerDownFocus = function( event ) {
  // focus element, if not touch, and its not an input or select
  if ( !this.options.accessibility || touchStartEvents[ event.type ] ||
      focusNodes[ event.target.nodeName ] ) {
    return;
  }
  var prevScrollY = window.pageYOffset;
  this.element.focus();
  // hack to fix scroll jump after focus, #76
  if ( window.pageYOffset != prevScrollY ) {
    window.scrollTo( window.pageXOffset, prevScrollY );
  }
};

// ----- move ----- //

Flickity.prototype.hasDragStarted = function( moveVector ) {
  return Math.abs( moveVector.x ) > 3;
};

// ----- up ----- //

Flickity.prototype.pointerUp = function( event, pointer ) {
  classie.remove( this.viewport, 'is-pointer-down' );
  this.dispatchEvent( 'pointerUp', event, [ pointer ] );
  this._dragPointerUp( event, pointer );
};

Flickity.prototype.pointerDone = function() {
  eventie.unbind( window, 'scroll', this );
  delete this.pointerDownScroll;
};

// -------------------------- dragging -------------------------- //

Flickity.prototype.dragStart = function( event, pointer ) {
  this.dragStartPosition = this.x;
  this.startAnimation();
  this.dispatchEvent( 'dragStart', event, [ pointer ] );
};

Flickity.prototype.dragMove = function( event, pointer, moveVector ) {
  preventDefaultEvent( event );

  this.previousDragX = this.dragX;
  // reverse if right-to-left
  var direction = this.options.rightToLeft ? -1 : 1;
  var dragX = this.dragStartPosition + moveVector.x * direction;

  if ( !this.options.wrapAround && this.cells.length ) {
    // slow drag
    var originBound = Math.max( -this.cells[0].target, this.dragStartPosition );
    dragX = dragX > originBound ? ( dragX + originBound ) * 0.5 : dragX;
    var endBound = Math.min( -this.getLastCell().target, this.dragStartPosition );
    dragX = dragX < endBound ? ( dragX + endBound ) * 0.5 : dragX;
  }

  this.dragX = dragX;

  this.dragMoveTime = new Date();
  this.dispatchEvent( 'dragMove', event, [ pointer, moveVector ] );
};

Flickity.prototype.dragEnd = function( event, pointer ) {
  if ( this.options.freeScroll ) {
    this.isFreeScrolling = true;
  }
  // set selectedIndex based on where flick will end up
  var index = this.dragEndRestingSelect();

  if ( this.options.freeScroll && !this.options.wrapAround ) {
    // if free-scroll & not wrap around
    // do not free-scroll if going outside of bounding cells
    // so bounding cells can attract slider, and keep it in bounds
    var restingX = this.getRestingPosition();
    this.isFreeScrolling = -restingX > this.cells[0].target &&
      -restingX < this.getLastCell().target;
  } else if ( !this.options.freeScroll && index == this.selectedIndex ) {
    // boost selection if selected index has not changed
    index += this.dragEndBoostSelect();
  }
  delete this.previousDragX;
  // apply selection
  // TODO refactor this, selecting here feels weird
  this.select( index );
  this.dispatchEvent( 'dragEnd', event, [ pointer ] );
};

Flickity.prototype.dragEndRestingSelect = function() {
  var restingX = this.getRestingPosition();
  // how far away from selected cell
  var distance = Math.abs( this.getCellDistance( -restingX, this.selectedIndex ) );
  // get closet resting going up and going down
  var positiveResting = this._getClosestResting( restingX, distance, 1 );
  var negativeResting = this._getClosestResting( restingX, distance, -1 );
  // use closer resting for wrap-around
  var index = positiveResting.distance < negativeResting.distance ?
    positiveResting.index : negativeResting.index;
  return index;
};

/**
 * given resting X and distance to selected cell
 * get the distance and index of the closest cell
 * @param {Number} restingX - estimated post-flick resting position
 * @param {Number} distance - distance to selected cell
 * @param {Integer} increment - +1 or -1, going up or down
 * @returns {Object} - { distance: {Number}, index: {Integer} }
 */
Flickity.prototype._getClosestResting = function( restingX, distance, increment ) {
  var index = this.selectedIndex;
  var minDistance = Infinity;
  var condition = this.options.contain && !this.options.wrapAround ?
    // if contain, keep going if distance is equal to minDistance
    function( d, md ) { return d <= md; } : function( d, md ) { return d < md; };
  while ( condition( distance, minDistance ) ) {
    // measure distance to next cell
    index += increment;
    minDistance = distance;
    distance = this.getCellDistance( -restingX, index );
    if ( distance === null ) {
      break;
    }
    distance = Math.abs( distance );
  }
  return {
    distance: minDistance,
    // selected was previous index
    index: index - increment
  };
};

/**
 * measure distance between x and a cell target
 * @param {Number} x
 * @param {Integer} index - cell index
 */
Flickity.prototype.getCellDistance = function( x, index ) {
  var len = this.cells.length;
  // wrap around if at least 2 cells
  var isWrapAround = this.options.wrapAround && len > 1;
  var cellIndex = isWrapAround ? utils.modulo( index, len ) : index;
  var cell = this.cells[ cellIndex ];
  if ( !cell ) {
    return null;
  }
  // add distance for wrap-around cells
  var wrap = isWrapAround ? this.slideableWidth * Math.floor( index / len ) : 0;
  return x - ( cell.target + wrap );
};

Flickity.prototype.dragEndBoostSelect = function() {
  // do not boost if no previousDragX or dragMoveTime
  if ( this.previousDragX === undefined || !this.dragMoveTime ||
    // or if drag was held for 100 ms
    new Date() - this.dragMoveTime > 100 ) {
    return 0;
  }

  var distance = this.getCellDistance( -this.dragX, this.selectedIndex );
  var delta = this.previousDragX - this.dragX;
  if ( distance > 0 && delta > 0 ) {
    // boost to next if moving towards the right, and positive velocity
    return 1;
  } else if ( distance < 0 && delta < 0 ) {
    // boost to previous if moving towards the left, and negative velocity
    return -1;
  }
  return 0;
};

// ----- staticClick ----- //

Flickity.prototype.staticClick = function( event, pointer ) {
  // get clickedCell, if cell was clicked
  var clickedCell = this.getParentCell( event.target );
  var cellElem = clickedCell && clickedCell.element;
  var cellIndex = clickedCell && utils.indexOf( this.cells, clickedCell );
  this.dispatchEvent( 'staticClick', event, [ pointer, cellElem, cellIndex ] );
};

// -----  ----- //

return Flickity;

}));

/*!
 * Tap listener v1.1.2
 * listens to taps
 * MIT license
 */

/*jshint browser: true, unused: true, undef: true, strict: true */

( function( window, factory ) {
  // universal module definition
  /*jshint strict: false*/ /*globals define, module, require */

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'tap-listener/tap-listener',[
      'unipointer/unipointer'
    ], function( Unipointer ) {
      return factory( window, Unipointer );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      window,
      require('unipointer')
    );
  } else {
    // browser global
    window.TapListener = factory(
      window,
      window.Unipointer
    );
  }

}( window, function factory( window, Unipointer ) {



// --------------------------  TapListener -------------------------- //

function TapListener( elem ) {
  this.bindTap( elem );
}

// inherit Unipointer & EventEmitter
TapListener.prototype = new Unipointer();

/**
 * bind tap event to element
 * @param {Element} elem
 */
TapListener.prototype.bindTap = function( elem ) {
  if ( !elem ) {
    return;
  }
  this.unbindTap();
  this.tapElement = elem;
  this._bindStartEvent( elem, true );
};

TapListener.prototype.unbindTap = function() {
  if ( !this.tapElement ) {
    return;
  }
  this._bindStartEvent( this.tapElement, true );
  delete this.tapElement;
};

var isPageOffset = window.pageYOffset !== undefined;
/**
 * pointer up
 * @param {Event} event
 * @param {Event or Touch} pointer
 */
TapListener.prototype.pointerUp = function( event, pointer ) {
  // ignore emulated mouse up clicks
  if ( this.isIgnoringMouseUp && event.type == 'mouseup' ) {
    return;
  }

  var pointerPoint = Unipointer.getPointerPoint( pointer );
  var boundingRect = this.tapElement.getBoundingClientRect();
  // standard or IE8 scroll positions
  var scrollX = isPageOffset ? window.pageXOffset : document.body.scrollLeft;
  var scrollY = isPageOffset ? window.pageYOffset : document.body.scrollTop;
  // calculate if pointer is inside tapElement
  var isInside = pointerPoint.x >= boundingRect.left + scrollX &&
    pointerPoint.x <= boundingRect.right + scrollX &&
    pointerPoint.y >= boundingRect.top + scrollY &&
    pointerPoint.y <= boundingRect.bottom + scrollY;
  // trigger callback if pointer is inside element
  if ( isInside ) {
    this.emitEvent( 'tap', [ event, pointer ] );
  }

  // set flag for emulated clicks 300ms after touchend
  if ( event.type != 'mouseup' ) {
    this.isIgnoringMouseUp = true;
    // reset flag after 300ms
    setTimeout( function() {
      delete this.isIgnoringMouseUp;
    }.bind( this ), 320 );
  }
};

TapListener.prototype.destroy = function() {
  this.pointerDone();
  this.unbindTap();
};

// -----  ----- //

return TapListener;

}));

// -------------------------- prev/next button -------------------------- //

( function( window, factory ) {
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'flickity/js/prev-next-button',[
      'eventie/eventie',
      './flickity',
      'tap-listener/tap-listener',
      'fizzy-ui-utils/utils'
    ], function( eventie, Flickity, TapListener, utils ) {
      return factory( window, eventie, Flickity, TapListener, utils );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      window,
      require('eventie'),
      require('./flickity'),
      require('tap-listener'),
      require('fizzy-ui-utils')
    );
  } else {
    // browser global
    factory(
      window,
      window.eventie,
      window.Flickity,
      window.TapListener,
      window.fizzyUIUtils
    );
  }

}( window, function factory( window, eventie, Flickity, TapListener, utils ) {



// ----- inline SVG support ----- //

var svgURI = 'http://www.w3.org/2000/svg';

// only check on demand, not on script load
var supportsInlineSVG = ( function() {
  var supports;
  function checkSupport() {
    if ( supports !== undefined ) {
      return supports;
    }
    var div = document.createElement('div');
    div.innerHTML = '<svg/>';
    supports = ( div.firstChild && div.firstChild.namespaceURI ) == svgURI;
    return supports;
  }
  return checkSupport;
})();

// -------------------------- PrevNextButton -------------------------- //

function PrevNextButton( direction, parent ) {
  this.direction = direction;
  this.parent = parent;
  this._create();
}

PrevNextButton.prototype = new TapListener();

PrevNextButton.prototype._create = function() {
  // properties
  this.isEnabled = true;
  this.isPrevious = this.direction == -1;
  var leftDirection = this.parent.options.rightToLeft ? 1 : -1;
  this.isLeft = this.direction == leftDirection;

  var element = this.element = document.createElement('button');
  element.className = 'flickity-prev-next-button';
  element.className += this.isPrevious ? ' previous' : ' next';
  // prevent button from submitting form http://stackoverflow.com/a/10836076/182183
  element.setAttribute( 'type', 'button' );
  // init as disabled
  this.disable();

  element.setAttribute( 'aria-label', this.isPrevious ? 'previous' : 'next' );

  Flickity.setUnselectable( element );
  // create arrow
  if ( supportsInlineSVG() ) {
    var svg = this.createSVG();
    element.appendChild( svg );
  } else {
    // SVG not supported, set button text
    this.setArrowText();
    element.className += ' no-svg';
  }
  // update on select
  var _this = this;
  this.onCellSelect = function() {
    _this.update();
  };
  this.parent.on( 'cellSelect', this.onCellSelect );
  // tap
  this.on( 'tap', this.onTap );
  // pointerDown
  this.on( 'pointerDown', function onPointerDown( button, event ) {
    _this.parent.childUIPointerDown( event );
  });
};

PrevNextButton.prototype.activate = function() {
  this.bindTap( this.element );
  // click events from keyboard
  eventie.bind( this.element, 'click', this );
  // add to DOM
  this.parent.element.appendChild( this.element );
};

PrevNextButton.prototype.deactivate = function() {
  // remove from DOM
  this.parent.element.removeChild( this.element );
  // do regular TapListener destroy
  TapListener.prototype.destroy.call( this );
  // click events from keyboard
  eventie.unbind( this.element, 'click', this );
};

PrevNextButton.prototype.createSVG = function() {
  var svg = document.createElementNS( svgURI, 'svg');
  svg.setAttribute( 'viewBox', '0 0 100 100' );
  var path = document.createElementNS( svgURI, 'path');
  var pathMovements = getArrowMovements( this.parent.options.arrowShape );
  path.setAttribute( 'd', pathMovements );
  path.setAttribute( 'class', 'arrow' );
  // rotate arrow
  if ( !this.isLeft ) {
    path.setAttribute( 'transform', 'translate(100, 100) rotate(180) ' );
  }
  svg.appendChild( path );
  return svg;
};

// get SVG path movmement
function getArrowMovements( shape ) {
  // use shape as movement if string
  if ( typeof shape == 'string' ) {
    return shape;
  }
  // create movement string
  return 'M ' + shape.x0 + ',50' +
    ' L ' + shape.x1 + ',' + ( shape.y1 + 50 ) +
    ' L ' + shape.x2 + ',' + ( shape.y2 + 50 ) +
    ' L ' + shape.x3 + ',50 ' +
    ' L ' + shape.x2 + ',' + ( 50 - shape.y2 ) +
    ' L ' + shape.x1 + ',' + ( 50 - shape.y1 ) +
    ' Z';
}

PrevNextButton.prototype.setArrowText = function() {
  var parentOptions = this.parent.options;
  var arrowText = this.isLeft ? parentOptions.leftArrowText : parentOptions.rightArrowText;
  utils.setText( this.element, arrowText );
};

PrevNextButton.prototype.onTap = function() {
  if ( !this.isEnabled ) {
    return;
  }
  this.parent.uiChange();
  var method = this.isPrevious ? 'previous' : 'next';
  this.parent[ method ]();
};

PrevNextButton.prototype.handleEvent = utils.handleEvent;

PrevNextButton.prototype.onclick = function() {
  // only allow clicks from keyboard
  var focused = document.activeElement;
  if ( focused && focused == this.element ) {
    this.onTap();
  }
};

// -----  ----- //

PrevNextButton.prototype.enable = function() {
  if ( this.isEnabled ) {
    return;
  }
  this.element.disabled = false;
  this.isEnabled = true;
};

PrevNextButton.prototype.disable = function() {
  if ( !this.isEnabled ) {
    return;
  }
  this.element.disabled = true;
  this.isEnabled = false;
};

PrevNextButton.prototype.update = function() {
  // index of first or last cell, if previous or next
  var cells = this.parent.cells;
  // enable is wrapAround and at least 2 cells
  if ( this.parent.options.wrapAround && cells.length > 1 ) {
    this.enable();
    return;
  }
  var lastIndex = cells.length ? cells.length - 1 : 0;
  var boundIndex = this.isPrevious ? 0 : lastIndex;
  var method = this.parent.selectedIndex == boundIndex ? 'disable' : 'enable';
  this[ method ]();
};

PrevNextButton.prototype.destroy = function() {
  this.deactivate();
};

// -------------------------- Flickity prototype -------------------------- //

utils.extend( Flickity.defaults, {
  prevNextButtons: true,
  leftArrowText: '‹',
  rightArrowText: '›',
  arrowShape: {
    x0: 10,
    x1: 60, y1: 50,
    x2: 70, y2: 40,
    x3: 30
  }
});

Flickity.createMethods.push('_createPrevNextButtons');

Flickity.prototype._createPrevNextButtons = function() {
  if ( !this.options.prevNextButtons ) {
    return;
  }

  this.prevButton = new PrevNextButton( -1, this );
  this.nextButton = new PrevNextButton( 1, this );

  this.on( 'activate', this.activatePrevNextButtons );
};

Flickity.prototype.activatePrevNextButtons = function() {
  this.prevButton.activate();
  this.nextButton.activate();
  this.on( 'deactivate', this.deactivatePrevNextButtons );
};

Flickity.prototype.deactivatePrevNextButtons = function() {
  this.prevButton.deactivate();
  this.nextButton.deactivate();
  this.off( 'deactivate', this.deactivatePrevNextButtons );
};

// --------------------------  -------------------------- //

Flickity.PrevNextButton = PrevNextButton;

return Flickity;

}));

( function( window, factory ) {
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'flickity/js/page-dots',[
      'eventie/eventie',
      './flickity',
      'tap-listener/tap-listener',
      'fizzy-ui-utils/utils'
    ], function( eventie, Flickity, TapListener, utils ) {
      return factory( window, eventie, Flickity, TapListener, utils );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      window,
      require('eventie'),
      require('./flickity'),
      require('tap-listener'),
      require('fizzy-ui-utils')
    );
  } else {
    // browser global
    factory(
      window,
      window.eventie,
      window.Flickity,
      window.TapListener,
      window.fizzyUIUtils
    );
  }

}( window, function factory( window, eventie, Flickity, TapListener, utils ) {

// -------------------------- PageDots -------------------------- //



function PageDots( parent ) {
  this.parent = parent;
  this._create();
}

PageDots.prototype = new TapListener();

PageDots.prototype._create = function() {
  // create holder element
  this.holder = document.createElement('ol');
  this.holder.className = 'flickity-page-dots';
  Flickity.setUnselectable( this.holder );
  // create dots, array of elements
  this.dots = [];
  // update on select
  var _this = this;
  this.onCellSelect = function() {
    _this.updateSelected();
  };
  this.parent.on( 'cellSelect', this.onCellSelect );
  // tap
  this.on( 'tap', this.onTap );
  // pointerDown
  this.on( 'pointerDown', function onPointerDown( button, event ) {
    _this.parent.childUIPointerDown( event );
  });
};

PageDots.prototype.activate = function() {
  this.setDots();
  this.bindTap( this.holder );
  // add to DOM
  this.parent.element.appendChild( this.holder );
};

PageDots.prototype.deactivate = function() {
  // remove from DOM
  this.parent.element.removeChild( this.holder );
  TapListener.prototype.destroy.call( this );
};

PageDots.prototype.setDots = function() {
  // get difference between number of cells and number of dots
  var delta = this.parent.cells.length - this.dots.length;
  if ( delta > 0 ) {
    this.addDots( delta );
  } else if ( delta < 0 ) {
    this.removeDots( -delta );
  }
};

PageDots.prototype.addDots = function( count ) {
  var fragment = document.createDocumentFragment();
  var newDots = [];
  while ( count ) {
    var dot = document.createElement('li');
    dot.className = 'dot';
    fragment.appendChild( dot );
    newDots.push( dot );
    count--;
  }
  this.holder.appendChild( fragment );
  this.dots = this.dots.concat( newDots );
};

PageDots.prototype.removeDots = function( count ) {
  // remove from this.dots collection
  var removeDots = this.dots.splice( this.dots.length - count, count );
  // remove from DOM
  for ( var i=0, len = removeDots.length; i < len; i++ ) {
    var dot = removeDots[i];
    this.holder.removeChild( dot );
  }
};

PageDots.prototype.updateSelected = function() {
  // remove selected class on previous
  if ( this.selectedDot ) {
    this.selectedDot.className = 'dot';
  }
  // don't proceed if no dots
  if ( !this.dots.length ) {
    return;
  }
  this.selectedDot = this.dots[ this.parent.selectedIndex ];
  this.selectedDot.className = 'dot is-selected';
};

PageDots.prototype.onTap = function( event ) {
  var target = event.target;
  // only care about dot clicks
  if ( target.nodeName != 'LI' ) {
    return;
  }

  this.parent.uiChange();
  var index = utils.indexOf( this.dots, target );
  this.parent.select( index );
};

PageDots.prototype.destroy = function() {
  this.deactivate();
};

Flickity.PageDots = PageDots;

// -------------------------- Flickity -------------------------- //

utils.extend( Flickity.defaults, {
  pageDots: true
});

Flickity.createMethods.push('_createPageDots');

Flickity.prototype._createPageDots = function() {
  if ( !this.options.pageDots ) {
    return;
  }
  this.pageDots = new PageDots( this );
  this.on( 'activate', this.activatePageDots );
  this.on( 'cellAddedRemoved', this.onCellAddedRemovedPageDots );
  this.on( 'deactivate', this.deactivatePageDots );
};

Flickity.prototype.activatePageDots = function() {
  this.pageDots.activate();
};

Flickity.prototype.onCellAddedRemovedPageDots = function() {
  this.pageDots.setDots();
};

Flickity.prototype.deactivatePageDots = function() {
  this.pageDots.deactivate();
};

// -----  ----- //

Flickity.PageDots = PageDots;

return Flickity;

}));

( function( window, factory ) {
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'flickity/js/player',[
      'eventEmitter/EventEmitter',
      'eventie/eventie',
      'fizzy-ui-utils/utils',
      './flickity'
    ], function( EventEmitter, eventie, utils, Flickity ) {
      return factory( EventEmitter, eventie, utils, Flickity );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      require('wolfy87-eventemitter'),
      require('eventie'),
      require('fizzy-ui-utils'),
      require('./flickity')
    );
  } else {
    // browser global
    factory(
      window.EventEmitter,
      window.eventie,
      window.fizzyUIUtils,
      window.Flickity
    );
  }

}( window, function factory( EventEmitter, eventie, utils, Flickity ) {



// -------------------------- Page Visibility -------------------------- //
// https://developer.mozilla.org/en-US/docs/Web/Guide/User_experience/Using_the_Page_Visibility_API

var hiddenProperty, visibilityEvent;
if ( 'hidden' in document ) {
  hiddenProperty = 'hidden';
  visibilityEvent = 'visibilitychange';
} else if ( 'webkitHidden' in document ) {
  hiddenProperty = 'webkitHidden';
  visibilityEvent = 'webkitvisibilitychange';
}

// -------------------------- Player -------------------------- //

function Player( parent ) {
  this.parent = parent;
  this.state = 'stopped';
  // visibility change event handler
  if ( visibilityEvent ) {
    var _this = this;
    this.onVisibilityChange = function() {
      _this.visibilityChange();
    };
  }
}

Player.prototype = new EventEmitter();

// start play
Player.prototype.play = function() {
  if ( this.state == 'playing' ) {
    return;
  }
  this.state = 'playing';
  // listen to visibility change
  if ( visibilityEvent ) {
    document.addEventListener( visibilityEvent, this.onVisibilityChange, false );
  }
  // start ticking
  this.tick();
};

Player.prototype.tick = function() {
  // do not tick if not playing
  if ( this.state != 'playing' ) {
    return;
  }

  var time = this.parent.options.autoPlay;
  // default to 3 seconds
  time = typeof time == 'number' ? time : 3000;
  var _this = this;
  // HACK: reset ticks if stopped and started within interval
  this.clear();
  this.timeout = setTimeout( function() {
    _this.parent.next( true );
    _this.tick();
  }, time );
};

Player.prototype.stop = function() {
  this.state = 'stopped';
  this.clear();
  // remove visibility change event
  if ( visibilityEvent ) {
    document.removeEventListener( visibilityEvent, this.onVisibilityChange, false );
  }
};

Player.prototype.clear = function() {
  clearTimeout( this.timeout );
};

Player.prototype.pause = function() {
  if ( this.state == 'playing' ) {
    this.state = 'paused';
    this.clear();
  }
};

Player.prototype.unpause = function() {
  // re-start play if in unpaused state
  if ( this.state == 'paused' ) {
    this.play();
  }
};

// pause if page visibility is hidden, unpause if visible
Player.prototype.visibilityChange = function() {
  var isHidden = document[ hiddenProperty ];
  this[ isHidden ? 'pause' : 'unpause' ]();
};

// -------------------------- Flickity -------------------------- //

utils.extend( Flickity.defaults, {
  pauseAutoPlayOnHover: true
});

Flickity.createMethods.push('_createPlayer');

Flickity.prototype._createPlayer = function() {
  this.player = new Player( this );

  this.on( 'activate', this.activatePlayer );
  this.on( 'uiChange', this.stopPlayer );
  this.on( 'pointerDown', this.stopPlayer );
  this.on( 'deactivate', this.deactivatePlayer );
};

Flickity.prototype.activatePlayer = function() {
  if ( !this.options.autoPlay ) {
    return;
  }
  this.player.play();
  eventie.bind( this.element, 'mouseenter', this );
  this.isMouseenterBound = true;
};

// Player API, don't hate the ... thanks I know where the door is

Flickity.prototype.playPlayer = function() {
  this.player.play();
};

Flickity.prototype.stopPlayer = function() {
  this.player.stop();
};

Flickity.prototype.pausePlayer = function() {
  this.player.pause();
};

Flickity.prototype.unpausePlayer = function() {
  this.player.unpause();
};

Flickity.prototype.deactivatePlayer = function() {
  this.player.stop();
  if ( this.isMouseenterBound ) {
    eventie.unbind( this.element, 'mouseenter', this );
    delete this.isMouseenterBound;
  }
};

// ----- mouseenter/leave ----- //

// pause auto-play on hover
Flickity.prototype.onmouseenter = function() {
  if ( !this.options.pauseAutoPlayOnHover ) {
    return;
  }
  this.player.pause();
  eventie.bind( this.element, 'mouseleave', this );
};

// resume auto-play on hover off
Flickity.prototype.onmouseleave = function() {
  this.player.unpause();
  eventie.unbind( this.element, 'mouseleave', this );
};

// -----  ----- //

Flickity.Player = Player;

return Flickity;

}));

( function( window, factory ) {
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'flickity/js/add-remove-cell',[
      './flickity',
      'fizzy-ui-utils/utils'
    ], function( Flickity, utils ) {
      return factory( window, Flickity, utils );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      window,
      require('./flickity'),
      require('fizzy-ui-utils')
    );
  } else {
    // browser global
    factory(
      window,
      window.Flickity,
      window.fizzyUIUtils
    );
  }

}( window, function factory( window, Flickity, utils ) {



// append cells to a document fragment
function getCellsFragment( cells ) {
  var fragment = document.createDocumentFragment();
  for ( var i=0, len = cells.length; i < len; i++ ) {
    var cell = cells[i];
    fragment.appendChild( cell.element );
  }
  return fragment;
}

// -------------------------- add/remove cell prototype -------------------------- //

/**
 * Insert, prepend, or append cells
 * @param {Element, Array, NodeList} elems
 * @param {Integer} index
 */
Flickity.prototype.insert = function( elems, index ) {
  var cells = this._makeCells( elems );
  if ( !cells || !cells.length ) {
    return;
  }
  var len = this.cells.length;
  // default to append
  index = index === undefined ? len : index;
  // add cells with document fragment
  var fragment = getCellsFragment( cells );
  // append to slider
  var isAppend = index == len;
  if ( isAppend ) {
    this.slider.appendChild( fragment );
  } else {
    var insertCellElement = this.cells[ index ].element;
    this.slider.insertBefore( fragment, insertCellElement );
  }
  // add to this.cells
  if ( index === 0 ) {
    // prepend, add to start
    this.cells = cells.concat( this.cells );
  } else if ( isAppend ) {
    // append, add to end
    this.cells = this.cells.concat( cells );
  } else {
    // insert in this.cells
    var endCells = this.cells.splice( index, len - index );
    this.cells = this.cells.concat( cells ).concat( endCells );
  }

  this._sizeCells( cells );

  var selectedIndexDelta = index > this.selectedIndex ? 0 : cells.length;
  this._cellAddedRemoved( index, selectedIndexDelta );
};

Flickity.prototype.append = function( elems ) {
  this.insert( elems, this.cells.length );
};

Flickity.prototype.prepend = function( elems ) {
  this.insert( elems, 0 );
};

/**
 * Remove cells
 * @param {Element, Array, NodeList} elems
 */
Flickity.prototype.remove = function( elems ) {
  var cells = this.getCells( elems );
  var selectedIndexDelta = 0;
  var i, len, cell;
  // calculate selectedIndexDelta, easier if done in seperate loop
  for ( i=0, len = cells.length; i < len; i++ ) {
    cell = cells[i];
    var wasBefore = utils.indexOf( this.cells, cell ) < this.selectedIndex;
    selectedIndexDelta -= wasBefore ? 1 : 0;
  }

  for ( i=0, len = cells.length; i < len; i++ ) {
    cell = cells[i];
    cell.remove();
    // remove item from collection
    utils.removeFrom( this.cells, cell );
  }

  if ( cells.length ) {
    // update stuff
    this._cellAddedRemoved( 0, selectedIndexDelta );
  }
};

// updates when cells are added or removed
Flickity.prototype._cellAddedRemoved = function( changedCellIndex, selectedIndexDelta ) {
  selectedIndexDelta = selectedIndexDelta || 0;
  this.selectedIndex += selectedIndexDelta;
  this.selectedIndex = Math.max( 0, Math.min( this.cells.length - 1, this.selectedIndex ) );

  this.emitEvent( 'cellAddedRemoved', [ changedCellIndex, selectedIndexDelta ] );
  this.cellChange( changedCellIndex, true );
};

/**
 * logic to be run after a cell's size changes
 * @param {Element} elem - cell's element
 */
Flickity.prototype.cellSizeChange = function( elem ) {
  var cell = this.getCell( elem );
  if ( !cell ) {
    return;
  }
  cell.getSize();

  var index = utils.indexOf( this.cells, cell );
  this.cellChange( index );
};

/**
 * logic any time a cell is changed: added, removed, or size changed
 * @param {Integer} changedCellIndex - index of the changed cell, optional
 */
Flickity.prototype.cellChange = function( changedCellIndex, isPositioningSlider ) {
  var prevSlideableWidth = this.slideableWidth;
  this._positionCells( changedCellIndex );
  this._getWrapShiftCells();
  this.setGallerySize();
  // position slider
  if ( this.options.freeScroll ) {
    // shift x by change in slideableWidth
    // TODO fix position shifts when prepending w/ freeScroll
    var deltaX = prevSlideableWidth - this.slideableWidth;
    this.x += deltaX * this.cellAlign;
    this.positionSlider();
  } else {
    // do not position slider after lazy load
    if ( isPositioningSlider ) {
      this.positionSliderAtSelected();
    }
    this.select( this.selectedIndex );
  }
};

// -----  ----- //

return Flickity;

}));

( function( window, factory ) {
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'flickity/js/lazyload',[
      'classie/classie',
      'eventie/eventie',
      './flickity',
      'fizzy-ui-utils/utils'
    ], function( classie, eventie, Flickity, utils ) {
      return factory( window, classie, eventie, Flickity, utils );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      window,
      require('desandro-classie'),
      require('eventie'),
      require('./flickity'),
      require('fizzy-ui-utils')
    );
  } else {
    // browser global
    factory(
      window,
      window.classie,
      window.eventie,
      window.Flickity,
      window.fizzyUIUtils
    );
  }

}( window, function factory( window, classie, eventie, Flickity, utils ) {
'use strict';

Flickity.createMethods.push('_createLazyload');

Flickity.prototype._createLazyload = function() {
  this.on( 'cellSelect', this.lazyLoad );
};

Flickity.prototype.lazyLoad = function() {
  var lazyLoad = this.options.lazyLoad;
  if ( !lazyLoad ) {
    return;
  }
  // get adjacent cells, use lazyLoad option for adjacent count
  var adjCount = typeof lazyLoad == 'number' ? lazyLoad : 0;
  var cellElems = this.getAdjacentCellElements( adjCount );
  // get lazy images in those cells
  var lazyImages = [];
  for ( var i=0, len = cellElems.length; i < len; i++ ) {
    var cellElem = cellElems[i];
    var lazyCellImages = getCellLazyImages( cellElem );
    lazyImages = lazyImages.concat( lazyCellImages );
  }
  // load lazy images
  for ( i=0, len = lazyImages.length; i < len; i++ ) {
    var img = lazyImages[i];
    new LazyLoader( img, this );
  }
};

function getCellLazyImages( cellElem ) {
  // check if cell element is lazy image
  if ( cellElem.nodeName == 'IMG' &&
    cellElem.getAttribute('data-flickity-lazyload') ) {
    return [ cellElem ];
  }
  // select lazy images in cell
  var imgs = cellElem.querySelectorAll('img[data-flickity-lazyload]');
  return utils.makeArray( imgs );
}

// -------------------------- LazyLoader -------------------------- //

/**
 * class to handle loading images
 */
function LazyLoader( img, flickity ) {
  this.img = img;
  this.flickity = flickity;
  this.load();
}

LazyLoader.prototype.handleEvent = utils.handleEvent;

LazyLoader.prototype.load = function() {
  eventie.bind( this.img, 'load', this );
  eventie.bind( this.img, 'error', this );
  // load image
  this.img.src = this.img.getAttribute('data-flickity-lazyload');
  // remove attr
  this.img.removeAttribute('data-flickity-lazyload');
};

LazyLoader.prototype.onload = function( event ) {
  this.complete( event, 'flickity-lazyloaded' );
};

LazyLoader.prototype.onerror = function( event ) {
  this.complete( event, 'flickity-lazyerror' );
};

LazyLoader.prototype.complete = function( event, className ) {
  // unbind events
  eventie.unbind( this.img, 'load', this );
  eventie.unbind( this.img, 'error', this );

  var cell = this.flickity.getParentCell( this.img );
  var cellElem = cell && cell.element;
  this.flickity.cellSizeChange( cellElem );

  classie.add( this.img, className );
  this.flickity.dispatchEvent( 'lazyLoad', event, cellElem );
};

// -----  ----- //

Flickity.LazyLoader = LazyLoader;

return Flickity;

}));

/*!
 * Flickity v1.2.1
 * Touch, responsive, flickable galleries
 *
 * Licensed GPLv3 for open source use
 * or Flickity Commercial License for commercial use
 *
 * http://flickity.metafizzy.co
 * Copyright 2015 Metafizzy
 */

( function( window, factory ) {
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'flickity/js/index',[
      './flickity',
      './drag',
      './prev-next-button',
      './page-dots',
      './player',
      './add-remove-cell',
      './lazyload'
    ], factory );
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      require('./flickity'),
      require('./drag'),
      require('./prev-next-button'),
      require('./page-dots'),
      require('./player'),
      require('./add-remove-cell'),
      require('./lazyload')
    );
  }

})( window, function factory( Flickity ) {
  /*jshint strict: false*/
  return Flickity;
});

/*!
 * Flickity asNavFor v1.0.4
 * enable asNavFor for Flickity
 */

/*jshint browser: true, undef: true, unused: true, strict: true*/

( function( window, factory ) {
  /*global define: false, module: false, require: false */
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'flickity-as-nav-for/as-nav-for',[
      'classie/classie',
      'flickity/js/index',
      'fizzy-ui-utils/utils'
    ], function( classie, Flickity, utils ) {
      return factory( window, classie, Flickity, utils );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      window,
      require('desandro-classie'),
      require('flickity'),
      require('fizzy-ui-utils')
    );
  } else {
    // browser global
    window.Flickity = factory(
      window,
      window.classie,
      window.Flickity,
      window.fizzyUIUtils
    );
  }

}( window, function factory( window, classie, Flickity, utils ) {



// -------------------------- asNavFor prototype -------------------------- //

// Flickity.defaults.asNavFor = null;

Flickity.createMethods.push('_createAsNavFor');

Flickity.prototype._createAsNavFor = function() {
  this.on( 'activate', this.activateAsNavFor );
  this.on( 'deactivate', this.deactivateAsNavFor );
  this.on( 'destroy', this.destroyAsNavFor );

  var asNavForOption = this.options.asNavFor;
  if ( !asNavForOption ) {
    return;
  }
  // HACK do async, give time for other flickity to be initalized
  var _this = this;
  setTimeout( function initNavCompanion() {
    _this.setNavCompanion( asNavForOption );
  });
};

Flickity.prototype.setNavCompanion = function( elem ) {
  elem = utils.getQueryElement( elem );
  var companion = Flickity.data( elem );
  // stop if no companion or companion is self
  if ( !companion || companion == this ) {
    return;
  }

  this.navCompanion = companion;
  // companion select
  var _this = this;
  this.onNavCompanionSelect = function() {
    _this.navCompanionSelect();
  };
  companion.on( 'cellSelect', this.onNavCompanionSelect );
  // click
  this.on( 'staticClick', this.onNavStaticClick );

  this.navCompanionSelect();
};

Flickity.prototype.navCompanionSelect = function() {
  if ( !this.navCompanion ) {
    return;
  }
  var index = this.navCompanion.selectedIndex;
  this.select( index );
  // set nav selected class
  this.removeNavSelectedElement();
  // stop if companion has more cells than this one
  if ( this.selectedIndex != index ) {
    return;
  }
  this.navSelectedElement = this.cells[ index ].element;
  classie.add( this.navSelectedElement, 'is-nav-selected' );
};

Flickity.prototype.activateAsNavFor = function() {
  this.navCompanionSelect();
};

Flickity.prototype.removeNavSelectedElement = function() {
  if ( !this.navSelectedElement ) {
    return;
  }
  classie.remove( this.navSelectedElement, 'is-nav-selected' );
  delete this.navSelectedElement;
};

Flickity.prototype.onNavStaticClick = function( event, pointer, cellElement, cellIndex ) {
  if ( typeof cellIndex == 'number' ) {
    this.navCompanion.select( cellIndex );
  }
};

Flickity.prototype.deactivateAsNavFor = function() {
  this.removeNavSelectedElement();
};

Flickity.prototype.destroyAsNavFor = function() {
  if ( !this.navCompanion ) {
    return;
  }
  this.navCompanion.off( 'cellSelect', this.onNavCompanionSelect );
  this.off( 'staticClick', this.onNavStaticClick );
  delete this.navCompanion;
};

// -----  ----- //

return Flickity;

}));

/*!
 * imagesLoaded v3.2.0
 * JavaScript is all like "You images are done yet or what?"
 * MIT License
 */

( function( window, factory ) { 'use strict';
  // universal module definition

  /*global define: false, module: false, require: false */

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( 'imagesloaded/imagesloaded',[
      'eventEmitter/EventEmitter',
      'eventie/eventie'
    ], function( EventEmitter, eventie ) {
      return factory( window, EventEmitter, eventie );
    });
  } else if ( typeof module == 'object' && module.exports ) {
    // CommonJS
    module.exports = factory(
      window,
      require('wolfy87-eventemitter'),
      require('eventie')
    );
  } else {
    // browser global
    window.imagesLoaded = factory(
      window,
      window.EventEmitter,
      window.eventie
    );
  }

})( window,

// --------------------------  factory -------------------------- //

function factory( window, EventEmitter, eventie ) {



var $ = window.jQuery;
var console = window.console;

// -------------------------- helpers -------------------------- //

// extend objects
function extend( a, b ) {
  for ( var prop in b ) {
    a[ prop ] = b[ prop ];
  }
  return a;
}

var objToString = Object.prototype.toString;
function isArray( obj ) {
  return objToString.call( obj ) == '[object Array]';
}

// turn element or nodeList into an array
function makeArray( obj ) {
  var ary = [];
  if ( isArray( obj ) ) {
    // use object if already an array
    ary = obj;
  } else if ( typeof obj.length == 'number' ) {
    // convert nodeList to array
    for ( var i=0; i < obj.length; i++ ) {
      ary.push( obj[i] );
    }
  } else {
    // array of single index
    ary.push( obj );
  }
  return ary;
}

  // -------------------------- imagesLoaded -------------------------- //

  /**
   * @param {Array, Element, NodeList, String} elem
   * @param {Object or Function} options - if function, use as callback
   * @param {Function} onAlways - callback function
   */
  function ImagesLoaded( elem, options, onAlways ) {
    // coerce ImagesLoaded() without new, to be new ImagesLoaded()
    if ( !( this instanceof ImagesLoaded ) ) {
      return new ImagesLoaded( elem, options, onAlways );
    }
    // use elem as selector string
    if ( typeof elem == 'string' ) {
      elem = document.querySelectorAll( elem );
    }

    this.elements = makeArray( elem );
    this.options = extend( {}, this.options );

    if ( typeof options == 'function' ) {
      onAlways = options;
    } else {
      extend( this.options, options );
    }

    if ( onAlways ) {
      this.on( 'always', onAlways );
    }

    this.getImages();

    if ( $ ) {
      // add jQuery Deferred object
      this.jqDeferred = new $.Deferred();
    }

    // HACK check async to allow time to bind listeners
    var _this = this;
    setTimeout( function() {
      _this.check();
    });
  }

  ImagesLoaded.prototype = new EventEmitter();

  ImagesLoaded.prototype.options = {};

  ImagesLoaded.prototype.getImages = function() {
    this.images = [];

    // filter & find items if we have an item selector
    for ( var i=0; i < this.elements.length; i++ ) {
      var elem = this.elements[i];
      this.addElementImages( elem );
    }
  };

  /**
   * @param {Node} element
   */
  ImagesLoaded.prototype.addElementImages = function( elem ) {
    // filter siblings
    if ( elem.nodeName == 'IMG' ) {
      this.addImage( elem );
    }
    // get background image on element
    if ( this.options.background === true ) {
      this.addElementBackgroundImages( elem );
    }

    // find children
    // no non-element nodes, #143
    var nodeType = elem.nodeType;
    if ( !nodeType || !elementNodeTypes[ nodeType ] ) {
      return;
    }
    var childImgs = elem.querySelectorAll('img');
    // concat childElems to filterFound array
    for ( var i=0; i < childImgs.length; i++ ) {
      var img = childImgs[i];
      this.addImage( img );
    }

    // get child background images
    if ( typeof this.options.background == 'string' ) {
      var children = elem.querySelectorAll( this.options.background );
      for ( i=0; i < children.length; i++ ) {
        var child = children[i];
        this.addElementBackgroundImages( child );
      }
    }
  };

  var elementNodeTypes = {
    1: true,
    9: true,
    11: true
  };

  ImagesLoaded.prototype.addElementBackgroundImages = function( elem ) {
    var style = getStyle( elem );
    // get url inside url("...")
    var reURL = /url\(['"]*([^'"\)]+)['"]*\)/gi;
    var matches = reURL.exec( style.backgroundImage );
    while ( matches !== null ) {
      var url = matches && matches[1];
      if ( url ) {
        this.addBackground( url, elem );
      }
      matches = reURL.exec( style.backgroundImage );
    }
  };

  // IE8
  var getStyle = window.getComputedStyle || function( elem ) {
    return elem.currentStyle;
  };

  /**
   * @param {Image} img
   */
  ImagesLoaded.prototype.addImage = function( img ) {
    var loadingImage = new LoadingImage( img );
    this.images.push( loadingImage );
  };

  ImagesLoaded.prototype.addBackground = function( url, elem ) {
    var background = new Background( url, elem );
    this.images.push( background );
  };

  ImagesLoaded.prototype.check = function() {
    var _this = this;
    this.progressedCount = 0;
    this.hasAnyBroken = false;
    // complete if no images
    if ( !this.images.length ) {
      this.complete();
      return;
    }

    function onProgress( image, elem, message ) {
      // HACK - Chrome triggers event before object properties have changed. #83
      setTimeout( function() {
        _this.progress( image, elem, message );
      });
    }

    for ( var i=0; i < this.images.length; i++ ) {
      var loadingImage = this.images[i];
      loadingImage.once( 'progress', onProgress );
      loadingImage.check();
    }
  };

  ImagesLoaded.prototype.progress = function( image, elem, message ) {
    this.progressedCount++;
    this.hasAnyBroken = this.hasAnyBroken || !image.isLoaded;
    // progress event
    this.emit( 'progress', this, image, elem );
    if ( this.jqDeferred && this.jqDeferred.notify ) {
      this.jqDeferred.notify( this, image );
    }
    // check if completed
    if ( this.progressedCount == this.images.length ) {
      this.complete();
    }

    if ( this.options.debug && console ) {
      console.log( 'progress: ' + message, image, elem );
    }
  };

  ImagesLoaded.prototype.complete = function() {
    var eventName = this.hasAnyBroken ? 'fail' : 'done';
    this.isComplete = true;
    this.emit( eventName, this );
    this.emit( 'always', this );
    if ( this.jqDeferred ) {
      var jqMethod = this.hasAnyBroken ? 'reject' : 'resolve';
      this.jqDeferred[ jqMethod ]( this );
    }
  };

  // --------------------------  -------------------------- //

  function LoadingImage( img ) {
    this.img = img;
  }

  LoadingImage.prototype = new EventEmitter();

  LoadingImage.prototype.check = function() {
    // If complete is true and browser supports natural sizes,
    // try to check for image status manually.
    var isComplete = this.getIsImageComplete();
    if ( isComplete ) {
      // report based on naturalWidth
      this.confirm( this.img.naturalWidth !== 0, 'naturalWidth' );
      return;
    }

    // If none of the checks above matched, simulate loading on detached element.
    this.proxyImage = new Image();
    eventie.bind( this.proxyImage, 'load', this );
    eventie.bind( this.proxyImage, 'error', this );
    // bind to image as well for Firefox. #191
    eventie.bind( this.img, 'load', this );
    eventie.bind( this.img, 'error', this );
    this.proxyImage.src = this.img.src;
  };

  LoadingImage.prototype.getIsImageComplete = function() {
    return this.img.complete && this.img.naturalWidth !== undefined;
  };

  LoadingImage.prototype.confirm = function( isLoaded, message ) {
    this.isLoaded = isLoaded;
    this.emit( 'progress', this, this.img, message );
  };

  // ----- events ----- //

  // trigger specified handler for event type
  LoadingImage.prototype.handleEvent = function( event ) {
    var method = 'on' + event.type;
    if ( this[ method ] ) {
      this[ method ]( event );
    }
  };

  LoadingImage.prototype.onload = function() {
    this.confirm( true, 'onload' );
    this.unbindEvents();
  };

  LoadingImage.prototype.onerror = function() {
    this.confirm( false, 'onerror' );
    this.unbindEvents();
  };

  LoadingImage.prototype.unbindEvents = function() {
    eventie.unbind( this.proxyImage, 'load', this );
    eventie.unbind( this.proxyImage, 'error', this );
    eventie.unbind( this.img, 'load', this );
    eventie.unbind( this.img, 'error', this );
  };

  // -------------------------- Background -------------------------- //

  function Background( url, element ) {
    this.url = url;
    this.element = element;
    this.img = new Image();
  }

  // inherit LoadingImage prototype
  Background.prototype = new LoadingImage();

  Background.prototype.check = function() {
    eventie.bind( this.img, 'load', this );
    eventie.bind( this.img, 'error', this );
    this.img.src = this.url;
    // check if image is already complete
    var isComplete = this.getIsImageComplete();
    if ( isComplete ) {
      this.confirm( this.img.naturalWidth !== 0, 'naturalWidth' );
      this.unbindEvents();
    }
  };

  Background.prototype.unbindEvents = function() {
    eventie.unbind( this.img, 'load', this );
    eventie.unbind( this.img, 'error', this );
  };

  Background.prototype.confirm = function( isLoaded, message ) {
    this.isLoaded = isLoaded;
    this.emit( 'progress', this, this.element, message );
  };

  // -------------------------- jQuery -------------------------- //

  ImagesLoaded.makeJQueryPlugin = function( jQuery ) {
    jQuery = jQuery || window.jQuery;
    if ( !jQuery ) {
      return;
    }
    // set local variable
    $ = jQuery;
    // $().imagesLoaded()
    $.fn.imagesLoaded = function( options, callback ) {
      var instance = new ImagesLoaded( this, options, callback );
      return instance.jqDeferred.promise( $(this) );
    };
  };
  // try making plugin
  ImagesLoaded.makeJQueryPlugin();

  // --------------------------  -------------------------- //

  return ImagesLoaded;

});

/*!
 * Flickity imagesLoaded v1.0.4
 * enables imagesLoaded option for Flickity
 */

/*jshint browser: true, strict: true, undef: true, unused: true */

( function( window, factory ) {
  /*global define: false, module: false, require: false */
  'use strict';
  // universal module definition

  if ( typeof define == 'function' && define.amd ) {
    // AMD
    define( [
      'flickity/js/index',
      'imagesloaded/imagesloaded'
    ], function( Flickity, imagesLoaded ) {
      return factory( window, Flickity, imagesLoaded );
    });
  } else if ( typeof exports == 'object' ) {
    // CommonJS
    module.exports = factory(
      window,
      require('flickity'),
      require('imagesloaded')
    );
  } else {
    // browser global
    window.Flickity = factory(
      window,
      window.Flickity,
      window.imagesLoaded
    );
  }

}( window, function factory( window, Flickity, imagesLoaded ) {
'use strict';

Flickity.createMethods.push('_createImagesLoaded');

Flickity.prototype._createImagesLoaded = function() {
  this.on( 'activate', this.imagesLoaded );
};

Flickity.prototype.imagesLoaded = function() {
  if ( !this.options.imagesLoaded ) {
    return;
  }
  var _this = this;
  function onImagesLoadedProgress( instance, image ) {
    var cell = _this.getParentCell( image.img );
    _this.cellSizeChange( cell && cell.element );
    if ( !_this.options.freeScroll ) {
      _this.positionSliderAtSelected();
    }
  }
  imagesLoaded( this.slider ).on( 'progress', onImagesLoadedProgress );
};

return Flickity;

}));


/**
 * Initiate the javascript slider "Lory"
 */
(function () {
  var sliderClass = "js-slider",
      sliderFrameClass = "js-slider__frame",
      sliderSlidesClass = "js-slider__slides",
      previousClass = 'js-slider__previous',
      nextClass = 'js-slider__next',
      gridClass = 'grid',
      sliders = document.getElementsByClassName(sliderClass);


  // initiate the slider if all content is loaded
  //document.addEventListener('DOMContentLoaded', function() {
    [].forEach.call(sliders, function (slider) {
      initiateSlider(slider);
    });
  //});

  // Function to initiate the slider
  function initiateSlider(slider) {
    var slides = slider.getElementsByClassName(sliderSlidesClass)[0];
    var sliderFrame = slider.getElementsByClassName(sliderFrameClass)[0];

    var flkty = new Flickity( slides, {
      cellAlign: 'left',
      contain: true,
      prevNextButtons: false,
      pageDots: false,
      imagesLoaded: true
    });

    // transfer grid class if required
    if(hasClass(slides, gridClass)) {
      removeClass(slides, gridClass);
      addClass(flkty.slider, gridClass);
    }

    // only add buttons if they are usefull
    var target = flkty.selectedCell.target;
    // enable / disable previous
    if(target !== flkty.cells[0].target || target !== flkty.getLastCell().target){
      addButtons();
    }

    function addButtons() {

      // Add navigation buttons to the slider
      // previous
      var previousLink = document.createElement('button');
      previousLink.href = "javascript:;";
      previousLink.text = "previous";
      addClass(previousLink, previousClass);
      slider.insertBefore(previousLink, sliderFrame);
      previousLink.addEventListener('click', function(){
        flkty.previous();
      });

      // next
      var nextLink = document.createElement('button');
      nextLink.href = "javascript:;";
      addClass(nextLink, nextClass);
      insertAfter(nextLink, sliderFrame);
      nextLink.text = "next";
      nextLink.addEventListener('click', function(){
        var target = flkty.selectedCell.target;
        if(target !== flkty.getLastCell().target) {
          flkty.next();
        }
      });

      function disablePreviousNext (flkty) {

        var target = flkty.selectedCell.target;
        // enable / disable previous
        if(target == flkty.cells[0].target){
          addClass(previousLink, previousClass+'--disabled');
        }else{
          removeClass(previousLink, previousClass+'--disabled');
        }
        // enable / disable next
        if(target == flkty.getLastCell().target){
          addClass(nextLink, nextClass+'--disabled');
        }else{
          removeClass(nextLink, nextClass+'--disabled');
        }
      }

      disablePreviousNext(flkty);

      // enable / disable the buttons
      flkty.on( 'cellSelect', function() {
        disablePreviousNext(flkty);
      });
    }
  }
})();

/**
 * Progressively enhance a select field
 */

// create global vl select function
vl.select = {};
vl.select.dress;
vl.select.setDisabledState;

(function () {

  var selectFields                      = document.querySelectorAll('[data-select]'),
      selectContentListItemActiveState  = 'select__cta--active',
      selectContentListItemFocusState   = 'select__cta--focus',
      selectContentListItemHiddenState  = 'select__cta--hidden',
      lastSelectId, lastContainer;

  vl.select.dress = function(selectField) {

    /*
    * Variables needed in Generate selects basted on original <select> elements
    */
    var arr                       = generateSelect(selectField),
        arrOptions                = arr[0],
        selectId                  = arr[1],
        selectContainer           = arr[2],
        originalSelectOption      = null,
        activeArrOptions          = arrOptions,
        selectDummyInput          = selectContainer.querySelector('.js-select__input'),
        selectContent             = selectContainer.querySelector('[data-content]'),
        selectContentInput        = selectContent.querySelector('[data-input]'),
        selectContentList         = selectContent.querySelector('[data-records]'),
        selectContentListItems    = selectContent.querySelectorAll('[data-record]'),
        selectFocusElems          = selectContainer.querySelectorAll('[data-focus]');

    /*
    * Events in select element
    */
    (selectContainer ? selectContainer.addEventListener('keyup', selectContainerKeyUpEventHandler) : null );
    (selectContainer ? selectContainer.addEventListener('keydown', selectContainerKeyDownEventHandler) : null );
    (selectDummyInput ? selectDummyInput.addEventListener('click', selectDummyInputClickEventHandler) : null );

    [].forEach.call(selectFocusElems, function(el){
      el.addEventListener('blur', selectFocusElemBlurHandler);
    });

    /*
    * Eventhandler | selectContainer keyUp
    */
    function selectContainerKeyUpEventHandler(e){
      e.preventDefault();

      var curOption, curOptionIndex;

      // Abort on tab and shift
      if(e.keyCode == 9 || e.keyCode == 16){
        return;
      }

      if(activeArrOptions.length <= 0){
        resetOptions();
      }else{
        setCurrentOption();
      }

      switch(e.keyCode){

        // "Backspace key"
        case 8:
          resetOptions();
          keyDefaultHandler();
        break;

        // "Esc key"
        case 27:
          setSelectState(false, selectContent, selectDummyInput, selectField);
          selectDummyInput.focus();
        break;

        // "Space key"
        case 32:
          if(selectContent.getAttribute('data-show') !== "true"){
            selectDummyInput.click();
          }
        break;

        // "Enter key"
        case 13:
          if(selectDummyInput.getAttribute('aria-expanded') !== "false" ){
            curOption.click();
            selectDummyInput.focus();
          }
        break;

        // "Arrow up key"
        case 38:
            keyUpHandler();
        break;

        // "Arrow down key"
        case 40:
            keyDownHandler();
        break;

        default:
          if(selectDummyInput.getAttribute('aria-expanded') !== "false"){
            keyDefaultHandler();
          }
        break;
      }

      function keyUpHandler(){
        e.preventDefault();

        if(selectContent.getAttribute('data-show') !== "true"){
          // Tonen bij arrow down en index één verhogen zodat je op dezelfde positie zit bij het openen
          setSelectState(true, selectContent, selectDummyInput, selectField);
          curOptionIndex++;
        }

        if(curOptionIndex > 0){
          curOptionIndex--;
          curOption.removeAttribute('data-selected');
          var el = selectContentList.querySelector('[data-record][data-index="'+curOptionIndex+'"]');
          el.setAttribute('data-selected','true');
          el.focus();
          if(selectContentInput !== null){
            selectContentInput.focus();
          }else{
            selectDummyInput.focus();
          }
        }
      }

      function keyDownHandler(){
        e.preventDefault();

        if(selectContent.getAttribute('data-show') !== "true"){
          // Tonen bij arrow down en index één minderen zodat je op dezelfde positie zit bij het openen
          setSelectState(true, selectContent, selectDummyInput, selectField);
          curOptionIndex--;
        }

        if(curOptionIndex < (activeArrOptions.length - 1)){
          curOptionIndex++;
          curOption.removeAttribute('data-selected');
          var el = selectContentList.querySelector('[data-record][data-index="'+curOptionIndex+'"]');
          el.setAttribute('data-selected','true');
          el.focus();
          if(selectContentInput !== null){
            selectContentInput.focus();
          }else{
            selectDummyInput.focus();
          }
        }
      }

      function keyDefaultHandler(){
        if(selectContentInput !== null){
          var val = selectContentInput.value, first; activeArrOptions = [];

          for(var item, i = 0; item = arrOptions[i]; i++) {
            var el = selectContentList.querySelector('[data-record][data-label="'+item+'"]');

            // Set visibility hidden of all items & Remove old index of all items & Remove old focus
            el.setAttribute('data-show', 'false');
            el.removeAttribute('data-index');
            el.removeAttribute('data-selected');

            // If substring is present in string show item and push to array
            if(item.toLowerCase().indexOf(val.toLowerCase() ) > -1){
              el.setAttribute('data-show', 'true');
              activeArrOptions.push(item);
            }
          }

          if(activeArrOptions.length > 0){
            setNoResultsFoundElement("hide", selectField, selectContentList);
            for(var opt, i = 0; opt = activeArrOptions[i]; i++) {
              selectContentList.querySelector('[data-record][data-label="'+opt+'"]').setAttribute('data-index', i);
            }
            // Set focus on first element
            if(selectContentList.querySelector('[data-record][data-index="0"]') !== null)
              selectContentList.querySelector('[data-record][data-index="0"]').setAttribute('data-selected','true');
          }else{
            setNoResultsFoundElement("show", selectField, selectContentList);
          }

        }
      }

      function setCurrentOption(){
        curOption = selectContentList.querySelector('[data-record][data-selected="true"]');
        (curOption == null ? curOption = selectContentList.querySelector('[data-record][data-index="0"]') : null);
        curOptionIndex = curOption.getAttribute('data-index');
      }

      function resetOptions(){
        for(var item, i = 0; item = arrOptions[i]; i++) {
          var el = selectContentList.querySelector('[data-record][data-label="'+item+'"]');
              el.removeAttribute('data-selected');
              el.setAttribute('data-show', 'true');
              el.setAttribute('data-index', i);
        }

        if(selectContentList.querySelector('[data-record][data-index="0"]') !== null)
          selectContentList.querySelector('[data-record][data-index="0"]').setAttribute('data-selected','true');


        activeArrOptions = arrOptions;
      }
    }

    /*
    * Eventhandler | selectContainer keyDown
    */
    function selectContainerKeyDownEventHandler(e){
      switch(e.keyCode){
        case 13: case 38: case 40:
        e.preventDefault();
        break;
      }
    }

    /*
    * Eventhandler | selectDummyInput Click
    */
    function selectDummyInputClickEventHandler(e){
      if(selectContent.getAttribute('data-show') === "false"){
        // Show select
        setSelectState(true, selectContent, selectDummyInput, selectField);
        // Set focus on search if present
        selectContentInput.focus();
        // Set selected option or first option active
        if(originalSelectOption !== null){
          selectContentList.querySelector('[data-record][data-label="'+originalSelectOption+'"]').setAttribute('data-selected', 'true');
        }else{
          if(selectContentList.querySelector('[data-record][data-index="0"]') !== null)
            selectContentList.querySelector('[data-record][data-index="0"]').setAttribute('data-selected','true');
        }
      }
      else{
        setSelectState(false, selectContent, selectDummyInput, selectField);
        selectDummyInput.focus();
      }
    }

    /*
    * selectFocusElemBlurHandler();
    * Used to check the focus state and close the select when focus is outside of the element
    */
    function selectFocusElemBlurHandler(e){
      window.setTimeout(function(){
        var parent = document.activeElement.closest('.js-select[data-id="' + selectId + '"]');
        if(parent === null){
          setSelectState(false, selectContent, selectDummyInput, selectField);
        }

      }, 200);
    };

    /*
    * Loop through dynamically generated records
    */
    [].forEach.call(selectContentListItems, function(item){
      item.addEventListener('click', function(e){
        var lbl = item.getAttribute('data-label');
        var val = item.getAttribute('data-value');

        // Set selected state to original select
        originalSelectOption = setOriginalSelectFieldOption(selectId, val);

        // Set label in dummy input
        selectDummyInput.innerHTML = lbl;

        // Close select
        setSelectState(false, selectContent, selectDummyInput, selectField);
        selectDummyInput.focus();

        // Remove active class of alle elements
        [].forEach.call(selectContentListItems, function(item2){
          removeClass(item2, selectContentListItemActiveState);
          item2.removeAttribute('data-selected');
        });

        // Add active class to selected element
        addClass(item, selectContentListItemActiveState);
      });
    });
  };

  /*
  * setDisabledState()
  * Sets disabled state of both native and generated select
  * @param selectField(object)
  * @param state(boolean)
  */
  vl.select.setDisabledState = function(selectField, state) {

    var selectContainer   = selectField.closest('.js-select');
    var selectDummyInput  = selectContainer.querySelector('.js-select__input');

    if(state){
      selectField.setAttribute('disabled', state);
      selectDummyInput.setAttribute('disabled', state);
    }else{
      selectField.removeAttribute('disabled');
      selectDummyInput.removeAttribute('disabled');
    }
  };

  /*
  * Loop through all select fields
  */
  [].forEach.call(selectFields, function(selectField) {
    vl.select.dress(selectField);
  });

  /*
  * setVisibilityAttributes()
  * Setting the general data attributes & aria tags
  */
  function setVisibilityAttributes(field, dataShow, ariaHidden){
      field.setAttribute('data-show',   dataShow);
      field.setAttribute('aria-hidden', ariaHidden);
  }

  /*
  * setSelectState()
  * Setting the general data attributes & aria tags of the generated select
  */
  function setSelectState(isShown, selectContent, selectDummyInput, selectField){
    if(isShown){
      var dataShow = true,
          ariaHidden = false,
          ariaExpanded = true;
    }else{
      var dataShow = false,
          ariaHidden = true,
          ariaExpanded = false;

          selectField.focus();
          window.setTimeout(function(){
            selectField.blur();
            if(selectField.getAttribute('data-has-error') == "true"){
              addClass(selectDummyInput, 'error');
            }else{
              removeClass(selectDummyInput, 'error');
            }
          }, 1);
    }

    selectContent.setAttribute('data-show', dataShow);
    selectContent.setAttribute('aria-hidden', ariaHidden);
    selectDummyInput.setAttribute('aria-expanded', ariaExpanded);
  }

  /*
  * generateSelect()
  * Generating the ehanced select
  */
  function generateSelect(selectField){
    // Hide normal select field
    addClass(selectField, 'u-visually-hidden');
    selectField.setAttribute('aria-hidden','true');

    var arr = [], uniqId = uniqueId();

    // Set selectContainer
    var selectContainer = selectField.closest('.js-select');

    // Get data-id or generate one
    if(selectField.hasAttribute('data-id')){
      uniqId = selectField.getAttribute('data-id');
      selectContainer.setAttribute('data-id', uniqId);
    }else{
      selectContainer.setAttribute('data-id', uniqId);
    }

    // Fake select field
    var selectDummyInput = document.createElement("button");
        selectDummyInput.setAttribute('type','button');
        selectDummyInput.setAttribute('data-focus', '');
        selectDummyInput.setAttribute('id', uniqId);
        selectDummyInput.setAttribute('aria-haspopup', 'true');
        selectDummyInput.setAttribute('aria-expanded', 'false');
        addClass(selectDummyInput, 'js-select__input');
        if(selectField.hasAttribute('disabled') && selectField.getAttribute('disabled') !== "false")
          selectDummyInput.setAttribute('disabled','true');

        selectContainer.insertBefore(selectDummyInput, selectContainer.firstChild);


    // Select Wrapper
    var selectWrapper = document.createElement("div");
        addClass(selectWrapper, 'select__wrapper');
        selectWrapper.setAttribute('data-content','');
        selectWrapper.setAttribute('aria-labelledby',uniqId);
        setVisibilityAttributes(selectWrapper, false, true);

        selectContainer.appendChild(selectWrapper);

        // Select Form Wrapper

        var selectForm = document.createElement("div");
            addClass(selectForm, 'select__form');

            selectWrapper.appendChild(selectForm);

            // Select Form Input
            var selectFormInput = document.createElement('input');
                selectFormInput.setAttribute('type','text');
                selectFormInput.setAttribute('autocomplete','off');
                addClass(selectFormInput, 'input-field');
                addClass(selectFormInput, 'input-field--block');
                selectFormInput.setAttribute('data-input','');
                selectFormInput.setAttribute('data-focus', '');
                selectFormInput.setAttribute('value','');
                selectFormInput.setAttribute('tabindex','-1');
                selectFormInput.setAttribute('aria-describedby', 'selectFormInputDescription' + uniqId);
                selectFormInput.setAttribute('aria-haspopup', 'listbox"');

                selectForm.appendChild(selectFormInput);

            var selectFormInputDescription = document.createElement('span');
                selectFormInputDescription.setAttribute('id','selectFormInputDescription' + uniqId);
                selectFormInputDescription.innerHTML = "U bevindt zich in de zoekfunctie van een dropdown menu in een formulier. Navigeer door de opties met ctrl + alt + pijltjes en selecteer met enter. Gebruik escape om de dropdown te sluiten.";
                addClass(selectFormInputDescription, 'u-visually-hidden');

                selectForm.appendChild(selectFormInputDescription);


        // Select List Wrapper
        var selectListWappper = document.createElement('div');
                addClass(selectListWappper,'select__list-wrapper');
                selectListWappper.setAttribute('role','listbox');

                selectWrapper.appendChild(selectListWappper);

                // Select List
                var selectList = document.createElement('section');
                    addClass(selectList, 'select__list');
                    selectList.setAttribute('data-records','');

                    selectListWappper.appendChild(selectList);

                    // Generate option groups based on optgroups in real select
                    var optgroups = selectField.querySelectorAll('optgroup');
                    if(optgroups.length > 0){
                      [].forEach.call(optgroups, function(optgroup){
                        var label = optgroup.getAttribute('label');
                        var selectOptionGroupWrapper = document.createElement('section');
                        addClass(selectOptionGroupWrapper, 'select__group');
                        selectOptionGroupWrapper.setAttribute('data-label', label);
                        selectOptionGroupWrapper.setAttribute('role', 'group');
                        selectList.appendChild(selectOptionGroupWrapper);

                        var selectOptionGroupTitle = document.createElement('h1');
                        selectOptionGroupTitle.innerHTML = label;

                        selectOptionGroupWrapper.appendChild(selectOptionGroupTitle);
                      });
                    }

                    // Generate list items based on options in real select
                    var i = 0;
                    [].forEach.call(selectField.options, function(opt){
                      var value = opt.value;
                      var label = opt.innerHTML;

                      // If item has "data-placeholder" it's used as a placeholder item
                      if(opt.hasAttribute('data-placeholder')){
                        selectDummyInput.innerHTML = label;
                      }else{
                        // SelectOption
                        var selectOption = document.createElement('div');
                        addClass(selectOption, 'select__item');

                        // Titel (button wrapper)
                        var selectOptionButton = document.createElement('button');
                            addClass(selectOptionButton, 'select__cta');
                            // If option is selected set the element active and change the label in the DummyInput
                            if(opt.selected){
                              addClass(selectOptionButton, selectContentListItemActiveState);
                              selectDummyInput.innerHTML = label;
                              selectOptionButton.setAttribute('aria-selected', true);
                            }else{
                              selectOptionButton.setAttribute('aria-selected', false);
                            }

                            var closestOptGroup = opt.closest('optgroup');

                            selectOptionButton.setAttribute('type', 'button');
                            selectOptionButton.setAttribute('data-index', i);
                            selectOptionButton.setAttribute('data-value', opt.value);
                            selectOptionButton.setAttribute('data-label', opt.label);
                            selectOptionButton.setAttribute('data-record','');
                            selectOptionButton.setAttribute('data-focus', '');
                            selectOptionButton.setAttribute('role', 'option');
                            selectOptionButton.setAttribute('tabindex','-1');

                            // Titel (span wrapper)
                            var selectOptionTitleSpan = document.createElement("span");
                                addClass(selectOptionTitleSpan, 'select__cta__title');
                                selectOptionTitleSpan.innerHTML = label;

                                // Appends
                                selectOptionButton.appendChild(selectOptionTitleSpan);
                            selectOption.appendChild(selectOptionButton);

                        // Assign to option group if available
                        if(closestOptGroup !== null){
                          var closestGeneratedOptGroup = selectList.querySelector('[data-label="' + closestOptGroup.getAttribute('label') + '"]')
                          closestGeneratedOptGroup.appendChild(selectOption);
                        }else{
                          selectList.appendChild(selectOption);
                        }

                        // Add to arrOptions array
                        arr.push(label);
                        i++;
                      }
                    });

      return [arr, uniqId, selectContainer];
  }

  /*
  * setNoResultsFoundElement()
  * Generate the "no results found" option
  */
  function setNoResultsFoundElement(state, selectField, selectContentList){
    switch(state){
      case "show":
        var prevEl = selectContentList.querySelector('[data-empty]');
        if(prevEl == null){
          var noResultsFoundElement = document.createElement('div');
              addClass(noResultsFoundElement, "select__item");

              selectContentList.appendChild(noResultsFoundElement);

              var noResultsFoundElementContent = document.createElement('div');
                  addClass(noResultsFoundElementContent, 'select__empty');
                  noResultsFoundElementContent.setAttribute('data-empty', '');
                  if(selectField.hasAttribute('data-search-empty')){
                    noResultsFoundElementContent.innerHTML = selectField.getAttribute('data-search-empty');
                  }else{
                    noResultsFoundElementContent.innerHTML = "No results found";
                  }

                  noResultsFoundElement.appendChild(noResultsFoundElementContent);
        }
      break;

      case "hide":
        var prevEl = selectContentList.querySelector('[data-empty]');
        if(prevEl !== null){
          removeElement(prevEl);
        }
      break;
    }
  }

  /*
  * setOriginalSelectFieldOption()
  * Setting the option in the hidden select field equal to the element selected in the generated select
  */
  function setOriginalSelectFieldOption(selectId, val){

    var sel = document.querySelector('.js-select[data-id="'+selectId+'"] select');
    for(var opt, j = 0; opt = sel.options[j]; j++) {
      if(opt.value == val) {
          sel.selectedIndex = j;
          return opt.label;
          break;
      }
    }
  }

})();

/**
 * Progressively enhance a multiselect field
 */

// create global vl multiselect function
vl.multiselect = {};
vl.multiselect.dress;
vl.multiselect.setDisabledState;

(function () {

  var selectFields                      = document.querySelectorAll('[data-multiselect]'),
      selectContentListItemActiveState  = 'select__cta--active',
      selectContentListItemFocusState   = 'select__cta--focus',
      selectContentListItemHiddenState  = 'select__cta--hidden',
      lastSelectId, lastContainer;

  vl.multiselect.dress = function(selectField) {

    /*
    * Variables needed in Generate selects basted on original <select> elements
    */
    var arr                       = generateSelect(selectField),
        arrOptions                = arr[0],
        selectId                  = arr[1],
        selectContainer           = arr[2],
        activeArrOptions          = arrOptions, // = options that are shown
        selectDummyInput          = selectContainer.querySelector('.js-select__input'),
        selectContent             = selectContainer.querySelector('[data-content]'),
        selectContentInput        = selectContent.querySelector('[data-input]'),
        selectContentList         = selectContent.querySelector('[data-records]'),
        selectContentListItems    = selectContent.querySelectorAll('[data-record]'),
        selectFocusElems          = selectContainer.querySelectorAll('[data-focus]'),
        selectedArrOptions        = generatePills(selectField, selectContainer, selectDummyInput, selectContentListItems, selectId); // = options that are selected


    /*
    * Events in select element
    */
    (selectContainer ? selectContainer.addEventListener('keyup', selectContainerKeyUpEventHandler) : null );
    (selectContainer ? selectContainer.addEventListener('keydown', selectContainerKeyDownEventHandler) : null );
    (selectDummyInput ? selectDummyInput.addEventListener('click', selectDummyInputClickEventHandler) : null );

    [].forEach.call(selectFocusElems, function(el){
      el.addEventListener('blur', selectFocusElemBlurHandler);
    });

    /*
    * Eventhandler | selectContainer keyUp
    */
    function selectContainerKeyUpEventHandler(e){
      e.preventDefault();

      var curOption, curOptionIndex;

      // Abort on tab and shift
      if(e.keyCode == 9 || e.keyCode == 16){
        return;
      }

      if(activeArrOptions.length <= 0){
        resetOptions();
      }else{
        setCurrentOption();
      }

      switch(e.keyCode){

        // "Backspace key"
        case 8:
          resetOptions();
          keyDefaultHandler();
        break;

        // "Esc key"
        case 27:
          setSelectState(false, selectContent, selectDummyInput);
          selectDummyInput.focus();
        break;

        // "Space key"
        case 32:
          if(selectContent.getAttribute('data-show') !== "true"){
            selectDummyInput.click();
          }
        break;

        // "Enter key"
        case 13:
        if(selectContent.getAttribute('data-show') !== "true"){
          setSelectState(true, selectContent, selectDummyInput);
          // Set focus
          if(selectContentInput !== null){
            selectContentInput.focus();
          }else{
            selectDummyInput.focus();
          }
        }else{
          curOption.click();
        }
        break;

        // "Arrow up key"
        case 38:
          if(activeArrOptions.length > 0)
            keyUpHandler();
        break;

        // "Arrow down key"
        case 40:
          if(activeArrOptions.length > 0)
            keyDownHandler();
        break;

        default:
          keyDefaultHandler();
        break;
      }

      function keyUpHandler(){
        e.preventDefault();

        if(selectContent.getAttribute('data-show') !== "true"){
          // Tonen bij arrow down en index één verhogen zodat je op dezelfde positie zit bij het openen
          setSelectState(true, selectContent, selectDummyInput);
          curOptionIndex++;
        }

        if(curOptionIndex > 0){
          curOptionIndex--;
          curOption.removeAttribute('data-selected');
          var el = selectContentList.querySelector('[data-record][data-index="'+curOptionIndex+'"]');
          el.setAttribute('data-selected','true');
          el.focus();
          if(selectContentInput !== null){
            selectContentInput.focus();
          }else{
            selectDummyInput.focus();
          }
        }
      }

      function keyDownHandler(){
        e.preventDefault();

        if(selectContent.getAttribute('data-show') !== "true"){
          // Tonen bij arrow down en index één minderen zodat je op dezelfde positie zit bij het openen
          setSelectState(true, selectContent, selectDummyInput);
          curOptionIndex--;
        }

        if(curOptionIndex < (activeArrOptions.length - 1)){
          curOptionIndex++;
          curOption.removeAttribute('data-selected');
          var el = selectContentList.querySelector('[data-record][data-index="'+curOptionIndex+'"]');
          el.setAttribute('data-selected','true');
          el.focus();
          if(selectContentInput !== null){
            selectContentInput.focus();
          }else{
            selectDummyInput.focus();
          }
        }
      }

      function keyDefaultHandler(){

        if(selectContentInput !== null){
          var val = selectContentInput.value, first; activeArrOptions = [];

          for(var item, i = 0; item = arrOptions[i]; i++) {
            var el = selectContentList.querySelector('[data-record][data-label="'+item+'"]');

            // Set visibility hidden of all items & Remove old index of all items & Remove old focus
            el.setAttribute('data-show', 'false');
            el.removeAttribute('data-index');
            el.removeAttribute('data-selected');

            // If substring is present in string show item and push to array
            if(item.toLowerCase().indexOf(val.toLowerCase() ) > -1){
              el.setAttribute('data-show', 'true');
              activeArrOptions.push(item);
            }
          }

          if(activeArrOptions.length > 0){
            setNoResultsFoundElement("hide", selectContainer, selectContentList);
            for(var opt, i = 0; opt = activeArrOptions[i]; i++) {
              selectContentList.querySelector('[data-record][data-label="'+opt+'"]').setAttribute('data-index', i);
            }
            // Set focus on first element
            if(selectContentList.querySelector('[data-record][data-index="0"]') !== null)
              selectContentList.querySelector('[data-record][data-index="0"]').setAttribute('data-selected','true');
          }else{
            setNoResultsFoundElement("show", selectContainer, selectContentList);
          }

        }
      }

      function setCurrentOption(){
        curOption = selectContentList.querySelector('[data-record][data-selected="true"]');
        (curOption == null ? curOption = selectContentList.querySelector('[data-record][data-index="0"]') : null);
        curOptionIndex = curOption.getAttribute('data-index');
      }

      function resetOptions(){

        for(var item, i = 0; item = arrOptions[i]; i++) {
          var el = selectContentList.querySelector('[data-record][data-label="'+item+'"]');
              el.removeAttribute('data-selected');
              el.setAttribute('data-show', 'true');
              el.setAttribute('data-index', i);
        }

        if(selectContentList.querySelector('[data-record][data-index="0"]') !== null)
          selectContentList.querySelector('[data-record][data-index="0"]').setAttribute('data-selected','true');

        activeArrOptions = arrOptions;
      }
    }

    /*
    * Eventhandler | selectContainer keyDown
    */
    function selectContainerKeyDownEventHandler(e){
      switch(e.keyCode){
        case 13: case 38: case 40:
        e.preventDefault();
        break;
      }
    }

    /*
    * Eventhandler | selectDummyInput Click
    */
    function selectDummyInputClickEventHandler(e){
      if(selectContent.getAttribute('data-show') === "false"){
        // Show select
        setSelectState(true, selectContent, selectDummyInput);
        // Set focus on search if present
        if(selectContentInput !== null){
          selectContentInput.focus();
        }
        // Select first element in generate records
        if(selectContentList.querySelector('[data-record][data-index="0"]') !== null)
          selectContentList.querySelector('[data-record][data-index="0"]').setAttribute('data-selected','true');
      }
      else{
        setSelectState(false, selectContent, selectDummyInput);
      }
    }

    /*
    * selectFocusElemBlurHandler();
    * Used to check the focus state and close the select when focus is outside of the element
    */
    function selectFocusElemBlurHandler(e){
      window.setTimeout(function(){
        var parent = document.activeElement.closest('.js-select[data-id="' + selectId + '"]');
        if(parent === null){
          setSelectState(false, selectContent, selectDummyInput);
        }
      }, 200);
    };

    /*
    * Loop through dynamically generated records
    */
    [].forEach.call(selectContentListItems, function(item){
      var lbl = item.getAttribute('data-label');
      var val = item.getAttribute('data-value');

      item.addEventListener('click', function(e){


        // toggle active class to selected element
        toggleClass(item, selectContentListItemActiveState);

        // Set selected state to original select
        selectedArrOptions = setOriginalSelectFieldOptions(selectId, selectContentListItems);

        // Generate pills
        generatePills(selectField, selectContainer, selectDummyInput, selectContentListItems, selectId);

        // Set focus
        (selectContentInput !== null ? selectContentInput.focus() : selectDummyInput.focus());

      });
    });
  };

  /*
  * Loop through all select fields
  */
  [].forEach.call(selectFields, function(selectField) {
    vl.multiselect.dress(selectField);
  });

  /*
  * setDisabledState()
  * Sets disabled state of both native and generated select
  * @param selectField(object)
  * @param state(boolean)
  */
  vl.multiselect.setDisabledState = function(selectField, state) {

    var selectContainer   = selectField.closest('.js-select');
    var selectDummyInput  = selectContainer.querySelector('.js-select__input');

    if(state){
      selectField.setAttribute('disabled', state);
      selectDummyInput.setAttribute('disabled', state);
    }else{
      selectField.removeAttribute('disabled');
      selectDummyInput.removeAttribute('disabled');
    }
  };

  /*
  * setVisibilityAttributes()
  * Setting the general data attributes & aria tags
  */
  function setVisibilityAttributes(field, dataShow, ariaHidden){
      field.setAttribute('data-show',   dataShow);
      field.setAttribute('aria-hidden', ariaHidden);
  }

  /*
  * setSelectState()
  * Setting the general data attributes & aria tags of the generated select
  */
  function setSelectState(isShown, selectContent, selectDummyInput){
    if(isShown)
      var dataShow = true, ariaHidden = false, ariaExpanded = true;
    else
      var dataShow = false, ariaHidden = true, ariaExpanded = false;

    selectContent.setAttribute('data-show', dataShow);
    selectContent.setAttribute('aria-hidden', ariaHidden);
    selectDummyInput.setAttribute('aria-expanded', ariaExpanded);
  }

  /*
  * generateSelect()
  * Generating the ehanced select
  */
  function generateSelect(selectField){
    // Hide normal select field
    addClass(selectField, 'u-visually-hidden');
    selectField.setAttribute('aria-hidden','true');

    var arr = [], uniqId = uniqueId();

    // Set selectContainer
    var selectContainer = selectField.closest('.js-select');

    // Get data-id or generate one
    if(selectField.hasAttribute('data-id')){
      uniqId = selectField.getAttribute('data-id');
      selectContainer.setAttribute('data-id', uniqId);
    }else{
      selectContainer.setAttribute('data-id', uniqId);
    }


    // Fake select field
    var selectDummyInput = document.createElement("button");
        selectDummyInput.setAttribute('type','button');
        selectDummyInput.setAttribute('data-focus', '');
        selectDummyInput.setAttribute('id', uniqId);
        selectDummyInput.setAttribute('aria-haspopup', 'true');
        selectDummyInput.setAttribute('aria-expanded', 'false');

        addClass(selectDummyInput, 'js-select__input');
        addClass(selectDummyInput, 'js-select__input--multi');
        if(selectField.hasAttribute('disabled') && selectField.getAttribute('disabled') !== "false")
          selectDummyInput.setAttribute('disabled','true');

        selectContainer.appendChild(selectDummyInput);


    // Select Wrapper
    var selectWrapper = document.createElement("div");
        addClass(selectWrapper, 'select__wrapper');
        selectWrapper.setAttribute('data-content','');
        selectWrapper.setAttribute('aria-labelledby',uniqId);
        setVisibilityAttributes(selectWrapper, false, true);

        selectContainer.appendChild(selectWrapper);

        // Select Form Wrapper
        var selectForm = document.createElement("div");
            addClass(selectForm, 'select__form');

            selectWrapper.appendChild(selectForm);

            // Select Form Input
            var selectFormInput = document.createElement('input');
                selectFormInput.setAttribute('type','text');
                selectFormInput.setAttribute('autocomplete','off');
                addClass(selectFormInput, 'input-field');
                addClass(selectFormInput, 'input-field--block');
                selectFormInput.setAttribute('data-input','');
                selectFormInput.setAttribute('data-focus', '');
                selectFormInput.setAttribute('value','');
                selectFormInput.setAttribute('tabindex','-1');
                selectFormInput.setAttribute('aria-describedby', 'selectFormInputDescription' + uniqId);
                selectFormInput.setAttribute('aria-haspopup', 'listbox"');

                selectForm.appendChild(selectFormInput);

                var selectFormInputDescription = document.createElement('span');
                    selectFormInputDescription.setAttribute('id','selectFormInputDescription' + uniqId);
                    selectFormInputDescription.innerHTML = "U bevindt zich in de zoekfunctie van een dropdown menu met multiselect in een formulier. Navigeer door de opties met ctrl + alt + pijltjes en selecteer met enter. Gebruik escape om de dropdown te sluiten.";
                    addClass(selectFormInputDescription, 'u-visually-hidden');

                    selectForm.appendChild(selectFormInputDescription);

        // Select List Wrapper
        var selectListWappper = document.createElement('div');
                addClass(selectListWappper,'select__list-wrapper');
                selectListWappper.setAttribute('role','listbox');

                selectWrapper.appendChild(selectListWappper);

                // Select List
                var selectList = document.createElement('section');
                    addClass(selectList, 'select__list');
                    selectList.setAttribute('data-records','');

                    selectListWappper.appendChild(selectList);

                    // Generate option groups based on optgroups in real select
                    var optgroups = selectField.querySelectorAll('optgroup');
                    if(optgroups.length > 0){
                      [].forEach.call(optgroups, function(optgroup){
                        var label = optgroup.getAttribute('label');
                        var selectOptionGroupWrapper = document.createElement('section');
                        addClass(selectOptionGroupWrapper, 'select__group');
                        selectOptionGroupWrapper.setAttribute('data-label', label);
                        selectOptionGroupWrapper.setAttribute('role', 'group');
                        selectList.appendChild(selectOptionGroupWrapper);

                        var selectOptionGroupTitle = document.createElement('h1');
                        selectOptionGroupTitle.innerHTML = label;

                        selectOptionGroupWrapper.appendChild(selectOptionGroupTitle);
                      });
                    }

                    // Generate list items based on options in real select
                    var i = 0;
                    [].forEach.call(selectField.options, function(opt){
                      var value = opt.value;
                      var label = opt.innerHTML;

                      // If item has "data-placeholder" it's used as a placeholder item
                      if(!opt.hasAttribute('data-placeholder')){
                        // SelectOption
                        var selectOption = document.createElement('div');
                        addClass(selectOption, 'select__item');

                        // Titel (button wrapper)
                        var selectOptionButton = document.createElement('button');
                            addClass(selectOptionButton, 'select__cta');
                            // If option is selected set the element active and change the label in the DummyInput
                            if(opt.selected){
                              addClass(selectOptionButton, selectContentListItemActiveState);
                              selectOptionButton.setAttribute('aria-selected', true);
                            }else{
                              selectOptionButton.setAttribute('aria-selected', false);
                            }

                            selectOptionButton.setAttribute('type', 'button');
                            selectOptionButton.setAttribute('data-index', i);
                            selectOptionButton.setAttribute('data-value', opt.value);
                            selectOptionButton.setAttribute('data-label', opt.label);
                            selectOptionButton.setAttribute('data-record','');
                            selectOptionButton.setAttribute('data-focus', '');
                            selectOptionButton.setAttribute('role', 'option');
                            selectOptionButton.setAttribute('tabindex','-1');

                            // Titel (span wrapper)
                            var selectOptionTitleSpan = document.createElement("span");
                                addClass(selectOptionTitleSpan, 'select__cta__title');
                                selectOptionTitleSpan.innerHTML = label;

                                // Appends
                                selectOptionButton.appendChild(selectOptionTitleSpan);
                            selectOption.appendChild(selectOptionButton);

                        // Assign to option group if available
                        var closestOptGroup = opt.closest('optgroup');
                        if(closestOptGroup !== null){
                          var closestGeneratedOptGroup = selectList.querySelector('[data-label="' + closestOptGroup.getAttribute('label') + '"]')
                          closestGeneratedOptGroup.appendChild(selectOption);
                        }else{
                          selectList.appendChild(selectOption);
                        }

                        // Add to arrOptions array
                        arr.push(label);
                        i++;
                      }
                    });

      return [arr, uniqId, selectContainer];
  }


  /*
  * generatePill()
  * Generating pills used in the multiselect input field
  */
  function generatePills(selectField, selectContainer, selectDummyInput, selectContentListItems, selectId){

    var activeOpts = selectContainer.querySelectorAll('.select__cta--active');
    var selectContentInput = selectContainer.querySelector('[data-input]');

    // Clear all elements
    selectDummyInput.innerHTML = "";

    // If present: generate pills based on active options
    if(activeOpts.length > 0){
      [].forEach.call(activeOpts, function(item){
          var pillWrapper = document.createElement("div");
          addClass(pillWrapper, 'pill'); addClass(pillWrapper, 'pill--closable');
          selectDummyInput.appendChild(pillWrapper);

            var pillSpan = document.createElement("span");
            pillSpan.innerHTML = item.getAttribute('data-label');
            pillWrapper.appendChild(pillSpan);

            var pillCta = document.createElement("a");
            addClass(pillCta, 'pill__close');
            pillCta.setAttribute('href', '#');
            pillCta.setAttribute('data-value', item.getAttribute('data-value'));
            pillCta.innerHTML = "close";

            // Remove pill on click/keyup(enter)
            pillCta.addEventListener('click', ctaEvent);
            pillCta.addEventListener('keyup', ctaKeydownEvent);

            function ctaKeydownEvent(e){
              if(e.keyCode === 13){
                ctaEvent(e);
              }
            }
            function ctaEvent(e){
              e.preventDefault();

              // Remove active class from corresponding item in select
              var correspondingItem = selectContainer.querySelector('[data-record][data-value="'+ item.getAttribute('data-value') +'"]');
              removeClass(correspondingItem, 'select__cta--active');

              // Generate pills
              generatePills(selectField, selectContainer, selectDummyInput, selectContentListItems, selectId);

              // Set selectfield options
              selectedArrOptions = setOriginalSelectFieldOptions(selectId, selectContentListItems);

              if(e.stopPropagation)
                e.stopPropagation();
              else
                e.cancelBubble = true;
            }
            pillWrapper.appendChild(pillCta);
      });
    }else{
      // Set placeholder or empty field
      var placeholder = selectField.querySelector('[data-placeholder]');
      if(placeholder !== null){
        selectDummyInput.innerHTML = placeholder.label;
      }else{
        selectDummyInput.innerHTML = "";
      }
    }
    return activeOpts;
  }

  /*
  * setNoResultsFoundElement()
  * Generate the "no results found" option
  */
  function setNoResultsFoundElement(state, selectContainer, selectContentList){
    switch(state){
      case "show":
        var prevEl = selectContentList.querySelector('[data-empty]');
        if(prevEl == null){
          var noResultsFoundElement = document.createElement('div');
              addClass(noResultsFoundElement, "select__item");

              selectContentList.appendChild(noResultsFoundElement);

              var noResultsFoundElementContent = document.createElement('div');
                  addClass(noResultsFoundElementContent, 'select__empty');
                  noResultsFoundElementContent.setAttribute('data-empty', '');
                  if(selectContainer.hasAttribute('data-search-empty')){
                    noResultsFoundElementContent.innerHTML = selectContainer.getAttribute('data-search-empty');
                  }else{
                    noResultsFoundElementContent.innerHTML = "No results found";
                  }

                  noResultsFoundElement.appendChild(noResultsFoundElementContent);
        }
      break;

      case "hide":
        var prevEl = selectContentList.querySelector('[data-empty]');
        if(prevEl !== null){
          removeElement(prevEl);
        }
      break;
    }
  }

  /*
  * setOriginalSelectFieldOptions()
  * Setting the options in the hidden select field equal to the element selected in the generated select
  */
  function setOriginalSelectFieldOptions(selectId, selectContentListItems){

    var sel = document.querySelector('.js-select[data-id="'+selectId+'"] select');
    var selectedArrOptions = [];
    var values = [];

    [].forEach.call(selectContentListItems, function(item){
      if(hasClass(item, selectContentListItemActiveState)){
        selectedArrOptions.push(item);
        values.push(item.getAttribute('data-value'));
      }
    });

      var opts = sel.options;
      for (var i = 0; i < opts.length; i++)
      {
          opts[i].selected = false;
          for (var j = 0; j < values.length; j++)
          {
              if (opts[i].value == values[j])
              {
                  opts[i].selected = true;
                  break;
              }
          }
      }

      // Return selected all options
      return selectedArrOptions;
  }

})();

/*! picturefill - v3.0.2 - 2016-02-12
 * https://scottjehl.github.io/picturefill/
 * Copyright (c) 2016 https://github.com/scottjehl/picturefill/blob/master/Authors.txt; Licensed MIT
 */
!function(a){var b=navigator.userAgent;a.HTMLPictureElement&&/ecko/.test(b)&&b.match(/rv\:(\d+)/)&&RegExp.$1<45&&addEventListener("resize",function(){var b,c=document.createElement("source"),d=function(a){var b,d,e=a.parentNode;"PICTURE"===e.nodeName.toUpperCase()?(b=c.cloneNode(),e.insertBefore(b,e.firstElementChild),setTimeout(function(){e.removeChild(b)})):(!a._pfLastSize||a.offsetWidth>a._pfLastSize)&&(a._pfLastSize=a.offsetWidth,d=a.sizes,a.sizes+=",100vw",setTimeout(function(){a.sizes=d}))},e=function(){var a,b=document.querySelectorAll("picture > img, img[srcset][sizes]");for(a=0;a<b.length;a++)d(b[a])},f=function(){clearTimeout(b),b=setTimeout(e,99)},g=a.matchMedia&&matchMedia("(orientation: landscape)"),h=function(){f(),g&&g.addListener&&g.addListener(f)};return c.srcset="data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==",/^[c|i]|d$/.test(document.readyState||"")?h():document.addEventListener("DOMContentLoaded",h),f}())}(window),function(a,b,c){"use strict";function d(a){return" "===a||"	"===a||"\n"===a||"\f"===a||"\r"===a}function e(b,c){var d=new a.Image;return d.onerror=function(){A[b]=!1,ba()},d.onload=function(){A[b]=1===d.width,ba()},d.src=c,"pending"}function f(){M=!1,P=a.devicePixelRatio,N={},O={},s.DPR=P||1,Q.width=Math.max(a.innerWidth||0,z.clientWidth),Q.height=Math.max(a.innerHeight||0,z.clientHeight),Q.vw=Q.width/100,Q.vh=Q.height/100,r=[Q.height,Q.width,P].join("-"),Q.em=s.getEmValue(),Q.rem=Q.em}function g(a,b,c,d){var e,f,g,h;return"saveData"===B.algorithm?a>2.7?h=c+1:(f=b-c,e=Math.pow(a-.6,1.5),g=f*e,d&&(g+=.1*e),h=a+g):h=c>1?Math.sqrt(a*b):a,h>c}function h(a){var b,c=s.getSet(a),d=!1;"pending"!==c&&(d=r,c&&(b=s.setRes(c),s.applySetCandidate(b,a))),a[s.ns].evaled=d}function i(a,b){return a.res-b.res}function j(a,b,c){var d;return!c&&b&&(c=a[s.ns].sets,c=c&&c[c.length-1]),d=k(b,c),d&&(b=s.makeUrl(b),a[s.ns].curSrc=b,a[s.ns].curCan=d,d.res||aa(d,d.set.sizes)),d}function k(a,b){var c,d,e;if(a&&b)for(e=s.parseSet(b),a=s.makeUrl(a),c=0;c<e.length;c++)if(a===s.makeUrl(e[c].url)){d=e[c];break}return d}function l(a,b){var c,d,e,f,g=a.getElementsByTagName("source");for(c=0,d=g.length;d>c;c++)e=g[c],e[s.ns]=!0,f=e.getAttribute("srcset"),f&&b.push({srcset:f,media:e.getAttribute("media"),type:e.getAttribute("type"),sizes:e.getAttribute("sizes")})}function m(a,b){function c(b){var c,d=b.exec(a.substring(m));return d?(c=d[0],m+=c.length,c):void 0}function e(){var a,c,d,e,f,i,j,k,l,m=!1,o={};for(e=0;e<h.length;e++)f=h[e],i=f[f.length-1],j=f.substring(0,f.length-1),k=parseInt(j,10),l=parseFloat(j),X.test(j)&&"w"===i?((a||c)&&(m=!0),0===k?m=!0:a=k):Y.test(j)&&"x"===i?((a||c||d)&&(m=!0),0>l?m=!0:c=l):X.test(j)&&"h"===i?((d||c)&&(m=!0),0===k?m=!0:d=k):m=!0;m||(o.url=g,a&&(o.w=a),c&&(o.d=c),d&&(o.h=d),d||c||a||(o.d=1),1===o.d&&(b.has1x=!0),o.set=b,n.push(o))}function f(){for(c(T),i="",j="in descriptor";;){if(k=a.charAt(m),"in descriptor"===j)if(d(k))i&&(h.push(i),i="",j="after descriptor");else{if(","===k)return m+=1,i&&h.push(i),void e();if("("===k)i+=k,j="in parens";else{if(""===k)return i&&h.push(i),void e();i+=k}}else if("in parens"===j)if(")"===k)i+=k,j="in descriptor";else{if(""===k)return h.push(i),void e();i+=k}else if("after descriptor"===j)if(d(k));else{if(""===k)return void e();j="in descriptor",m-=1}m+=1}}for(var g,h,i,j,k,l=a.length,m=0,n=[];;){if(c(U),m>=l)return n;g=c(V),h=[],","===g.slice(-1)?(g=g.replace(W,""),e()):f()}}function n(a){function b(a){function b(){f&&(g.push(f),f="")}function c(){g[0]&&(h.push(g),g=[])}for(var e,f="",g=[],h=[],i=0,j=0,k=!1;;){if(e=a.charAt(j),""===e)return b(),c(),h;if(k){if("*"===e&&"/"===a[j+1]){k=!1,j+=2,b();continue}j+=1}else{if(d(e)){if(a.charAt(j-1)&&d(a.charAt(j-1))||!f){j+=1;continue}if(0===i){b(),j+=1;continue}e=" "}else if("("===e)i+=1;else if(")"===e)i-=1;else{if(","===e){b(),c(),j+=1;continue}if("/"===e&&"*"===a.charAt(j+1)){k=!0,j+=2;continue}}f+=e,j+=1}}}function c(a){return k.test(a)&&parseFloat(a)>=0?!0:l.test(a)?!0:"0"===a||"-0"===a||"+0"===a?!0:!1}var e,f,g,h,i,j,k=/^(?:[+-]?[0-9]+|[0-9]*\.[0-9]+)(?:[eE][+-]?[0-9]+)?(?:ch|cm|em|ex|in|mm|pc|pt|px|rem|vh|vmin|vmax|vw)$/i,l=/^calc\((?:[0-9a-z \.\+\-\*\/\(\)]+)\)$/i;for(f=b(a),g=f.length,e=0;g>e;e++)if(h=f[e],i=h[h.length-1],c(i)){if(j=i,h.pop(),0===h.length)return j;if(h=h.join(" "),s.matchesMedia(h))return j}return"100vw"}b.createElement("picture");var o,p,q,r,s={},t=!1,u=function(){},v=b.createElement("img"),w=v.getAttribute,x=v.setAttribute,y=v.removeAttribute,z=b.documentElement,A={},B={algorithm:""},C="data-pfsrc",D=C+"set",E=navigator.userAgent,F=/rident/.test(E)||/ecko/.test(E)&&E.match(/rv\:(\d+)/)&&RegExp.$1>35,G="currentSrc",H=/\s+\+?\d+(e\d+)?w/,I=/(\([^)]+\))?\s*(.+)/,J=a.picturefillCFG,K="position:absolute;left:0;visibility:hidden;display:block;padding:0;border:none;font-size:1em;width:1em;overflow:hidden;clip:rect(0px, 0px, 0px, 0px)",L="font-size:100%!important;",M=!0,N={},O={},P=a.devicePixelRatio,Q={px:1,"in":96},R=b.createElement("a"),S=!1,T=/^[ \t\n\r\u000c]+/,U=/^[, \t\n\r\u000c]+/,V=/^[^ \t\n\r\u000c]+/,W=/[,]+$/,X=/^\d+$/,Y=/^-?(?:[0-9]+|[0-9]*\.[0-9]+)(?:[eE][+-]?[0-9]+)?$/,Z=function(a,b,c,d){a.addEventListener?a.addEventListener(b,c,d||!1):a.attachEvent&&a.attachEvent("on"+b,c)},$=function(a){var b={};return function(c){return c in b||(b[c]=a(c)),b[c]}},_=function(){var a=/^([\d\.]+)(em|vw|px)$/,b=function(){for(var a=arguments,b=0,c=a[0];++b in a;)c=c.replace(a[b],a[++b]);return c},c=$(function(a){return"return "+b((a||"").toLowerCase(),/\band\b/g,"&&",/,/g,"||",/min-([a-z-\s]+):/g,"e.$1>=",/max-([a-z-\s]+):/g,"e.$1<=",/calc([^)]+)/g,"($1)",/(\d+[\.]*[\d]*)([a-z]+)/g,"($1 * e.$2)",/^(?!(e.[a-z]|[0-9\.&=|><\+\-\*\(\)\/])).*/gi,"")+";"});return function(b,d){var e;if(!(b in N))if(N[b]=!1,d&&(e=b.match(a)))N[b]=e[1]*Q[e[2]];else try{N[b]=new Function("e",c(b))(Q)}catch(f){}return N[b]}}(),aa=function(a,b){return a.w?(a.cWidth=s.calcListLength(b||"100vw"),a.res=a.w/a.cWidth):a.res=a.d,a},ba=function(a){if(t){var c,d,e,f=a||{};if(f.elements&&1===f.elements.nodeType&&("IMG"===f.elements.nodeName.toUpperCase()?f.elements=[f.elements]:(f.context=f.elements,f.elements=null)),c=f.elements||s.qsa(f.context||b,f.reevaluate||f.reselect?s.sel:s.selShort),e=c.length){for(s.setupRun(f),S=!0,d=0;e>d;d++)s.fillImg(c[d],f);s.teardownRun(f)}}};o=a.console&&console.warn?function(a){console.warn(a)}:u,G in v||(G="src"),A["image/jpeg"]=!0,A["image/gif"]=!0,A["image/png"]=!0,A["image/svg+xml"]=b.implementation.hasFeature("http://www.w3.org/TR/SVG11/feature#Image","1.1"),s.ns=("pf"+(new Date).getTime()).substr(0,9),s.supSrcset="srcset"in v,s.supSizes="sizes"in v,s.supPicture=!!a.HTMLPictureElement,s.supSrcset&&s.supPicture&&!s.supSizes&&!function(a){v.srcset="data:,a",a.src="data:,a",s.supSrcset=v.complete===a.complete,s.supPicture=s.supSrcset&&s.supPicture}(b.createElement("img")),s.supSrcset&&!s.supSizes?!function(){var a="data:image/gif;base64,R0lGODlhAgABAPAAAP///wAAACH5BAAAAAAALAAAAAACAAEAAAICBAoAOw==",c="data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==",d=b.createElement("img"),e=function(){var a=d.width;2===a&&(s.supSizes=!0),q=s.supSrcset&&!s.supSizes,t=!0,setTimeout(ba)};d.onload=e,d.onerror=e,d.setAttribute("sizes","9px"),d.srcset=c+" 1w,"+a+" 9w",d.src=c}():t=!0,s.selShort="picture>img,img[srcset]",s.sel=s.selShort,s.cfg=B,s.DPR=P||1,s.u=Q,s.types=A,s.setSize=u,s.makeUrl=$(function(a){return R.href=a,R.href}),s.qsa=function(a,b){return"querySelector"in a?a.querySelectorAll(b):[]},s.matchesMedia=function(){return a.matchMedia&&(matchMedia("(min-width: 0.1em)")||{}).matches?s.matchesMedia=function(a){return!a||matchMedia(a).matches}:s.matchesMedia=s.mMQ,s.matchesMedia.apply(this,arguments)},s.mMQ=function(a){return a?_(a):!0},s.calcLength=function(a){var b=_(a,!0)||!1;return 0>b&&(b=!1),b},s.supportsType=function(a){return a?A[a]:!0},s.parseSize=$(function(a){var b=(a||"").match(I);return{media:b&&b[1],length:b&&b[2]}}),s.parseSet=function(a){return a.cands||(a.cands=m(a.srcset,a)),a.cands},s.getEmValue=function(){var a;if(!p&&(a=b.body)){var c=b.createElement("div"),d=z.style.cssText,e=a.style.cssText;c.style.cssText=K,z.style.cssText=L,a.style.cssText=L,a.appendChild(c),p=c.offsetWidth,a.removeChild(c),p=parseFloat(p,10),z.style.cssText=d,a.style.cssText=e}return p||16},s.calcListLength=function(a){if(!(a in O)||B.uT){var b=s.calcLength(n(a));O[a]=b?b:Q.width}return O[a]},s.setRes=function(a){var b;if(a){b=s.parseSet(a);for(var c=0,d=b.length;d>c;c++)aa(b[c],a.sizes)}return b},s.setRes.res=aa,s.applySetCandidate=function(a,b){if(a.length){var c,d,e,f,h,k,l,m,n,o=b[s.ns],p=s.DPR;if(k=o.curSrc||b[G],l=o.curCan||j(b,k,a[0].set),l&&l.set===a[0].set&&(n=F&&!b.complete&&l.res-.1>p,n||(l.cached=!0,l.res>=p&&(h=l))),!h)for(a.sort(i),f=a.length,h=a[f-1],d=0;f>d;d++)if(c=a[d],c.res>=p){e=d-1,h=a[e]&&(n||k!==s.makeUrl(c.url))&&g(a[e].res,c.res,p,a[e].cached)?a[e]:c;break}h&&(m=s.makeUrl(h.url),o.curSrc=m,o.curCan=h,m!==k&&s.setSrc(b,h),s.setSize(b))}},s.setSrc=function(a,b){var c;a.src=b.url,"image/svg+xml"===b.set.type&&(c=a.style.width,a.style.width=a.offsetWidth+1+"px",a.offsetWidth+1&&(a.style.width=c))},s.getSet=function(a){var b,c,d,e=!1,f=a[s.ns].sets;for(b=0;b<f.length&&!e;b++)if(c=f[b],c.srcset&&s.matchesMedia(c.media)&&(d=s.supportsType(c.type))){"pending"===d&&(c=d),e=c;break}return e},s.parseSets=function(a,b,d){var e,f,g,h,i=b&&"PICTURE"===b.nodeName.toUpperCase(),j=a[s.ns];(j.src===c||d.src)&&(j.src=w.call(a,"src"),j.src?x.call(a,C,j.src):y.call(a,C)),(j.srcset===c||d.srcset||!s.supSrcset||a.srcset)&&(e=w.call(a,"srcset"),j.srcset=e,h=!0),j.sets=[],i&&(j.pic=!0,l(b,j.sets)),j.srcset?(f={srcset:j.srcset,sizes:w.call(a,"sizes")},j.sets.push(f),g=(q||j.src)&&H.test(j.srcset||""),g||!j.src||k(j.src,f)||f.has1x||(f.srcset+=", "+j.src,f.cands.push({url:j.src,d:1,set:f}))):j.src&&j.sets.push({srcset:j.src,sizes:null}),j.curCan=null,j.curSrc=c,j.supported=!(i||f&&!s.supSrcset||g&&!s.supSizes),h&&s.supSrcset&&!j.supported&&(e?(x.call(a,D,e),a.srcset=""):y.call(a,D)),j.supported&&!j.srcset&&(!j.src&&a.src||a.src!==s.makeUrl(j.src))&&(null===j.src?a.removeAttribute("src"):a.src=j.src),j.parsed=!0},s.fillImg=function(a,b){var c,d=b.reselect||b.reevaluate;a[s.ns]||(a[s.ns]={}),c=a[s.ns],(d||c.evaled!==r)&&((!c.parsed||b.reevaluate)&&s.parseSets(a,a.parentNode,b),c.supported?c.evaled=r:h(a))},s.setupRun=function(){(!S||M||P!==a.devicePixelRatio)&&f()},s.supPicture?(ba=u,s.fillImg=u):!function(){var c,d=a.attachEvent?/d$|^c/:/d$|^c|^i/,e=function(){var a=b.readyState||"";f=setTimeout(e,"loading"===a?200:999),b.body&&(s.fillImgs(),c=c||d.test(a),c&&clearTimeout(f))},f=setTimeout(e,b.body?9:99),g=function(a,b){var c,d,e=function(){var f=new Date-d;b>f?c=setTimeout(e,b-f):(c=null,a())};return function(){d=new Date,c||(c=setTimeout(e,b))}},h=z.clientHeight,i=function(){M=Math.max(a.innerWidth||0,z.clientWidth)!==Q.width||z.clientHeight!==h,h=z.clientHeight,M&&s.fillImgs()};Z(a,"resize",g(i,99)),Z(b,"readystatechange",e)}(),s.picturefill=ba,s.fillImgs=ba,s.teardownRun=u,ba._=s,a.picturefillCFG={pf:s,push:function(a){var b=a.shift();"function"==typeof s[b]?s[b].apply(s,a):(B[b]=a[0],S&&s.fillImgs({reselect:!0}))}};for(;J&&J.length;)a.picturefillCFG.push(J.shift());a.picturefill=ba,"object"==typeof module&&"object"==typeof module.exports?module.exports=ba:"function"==typeof define&&define.amd&&define("picturefill",function(){return ba}),s.supPicture||(A["image/webp"]=e("image/webp","data:image/webp;base64,UklGRkoAAABXRUJQVlA4WAoAAAAQAAAAAAAAAAAAQUxQSAwAAAABBxAR/Q9ERP8DAABWUDggGAAAADABAJ0BKgEAAQADADQlpAADcAD++/1QAA=="))}(window,document);