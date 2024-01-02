"use strict";var e=this&&this.__assign||function(){return e=Object.assign||function(e){for(var n,t=1,r=arguments.length;t<r;t++)for(var i in n=arguments[t])Object.prototype.hasOwnProperty.call(n,i)&&(e[i]=n[i]);return e},e.apply(this,arguments)},n=[/Mobile/i],t=[{name:"ie",includes:[/Trident\/[.\w]+.+?rv:((\d+)[.\w]*)/i,/MSIE ((\d+\.\d+)[.\w]*)/i],excludes:n},{name:"edge",includes:[/Edg\/((\d+)[.\w]*)/i,/Edge\/((\d+)[.\w]*)/i,/EdgA\/((\d+)[.\w]*)/i]},{name:"chrome",includes:[/Chrome\/((\d+)[.\w]*)/i],excludes:n},{name:"safari",includes:[/Version\/((\d+\.\d+)[.\w]*).+Safari/i],excludes:n},{name:"firefox",includes:[/Firefox\/((\d+\.\d+)[.\w]*)/i],excludes:n},{name:"opera",includes:[/OPR\/((\d+)[.\w]*)/i,/Presto\/[.\w]+.+Version\/((\d+\.\d)[.\w]*)/i,/Opera\/((\d+\.\d)[.\w]*)/i],excludes:[/Mobile|Mobi|Tablet/i]},{name:"android",includes:[/wv.+?Chrome\/((\d+)[.\w]*)/i]},{name:"ios_saf",includes:[/(iPad|iPhone).+OS ((\d+_\d+)\w*)/i]},{name:"and_chr",includes:[/Chrome\/((\d+)[.\w]*).+Mobile/i],excludes:[/wv/i]},{name:"and_ff",includes:[/Mobile;.+Firefox\/((\d+\.\d+)[.\w]*)/i],excludes:[/wv/i]}];window.obsolete=function(n,r){function i(e,n){var t=/\d+/g,r=/\d+/g,i=/^(\d+)(\.\d+)*$/;for([e,n].forEach((function(n){if(!i.test(n))throw new Error("Parameter `version` `".concat(e,"` isn't a semantic version."))}));;){var o=t.exec(e),a=r.exec(n);if(o&&!a)return 0===Number(o[0])?0:1;if(!o&&a)return 0===Number(a[0])?0:-1;if(o&&a){if(Number(o[0])>Number(a[0]))return 1;if(Number(o[0])<Number(a[0]))return-1}if(!o&&!a)return 0}}if(r=e({},r),!n.length)throw new Error("Parameter `browsers` is empty");var o=function(e,n){var o=[];if(t.forEach((function(n){var t;if(!n.excludes||!n.excludes.some((function(n){return n.exec(e)})))for(var r=0;r<n.includes.length;r++)if(t=n.includes[r].exec(e)){o.push({name:n.name,version:t[1].replace(/_/g,"."),primaryVersion:t[2].replace(/_/g,".")});break}})),!o.length)return!1;var a=/(\w+) (([\d.]+)(?:-[\d.]+)?)/,d={};n.map((function(e){var n=a.exec({"op_mini all":"op_mini 0","safari TP":"safari 99"}[e]||e);return{name:n[1],version:n[2],primaryVersion:n[3]}})).forEach((function(e){d[e.name]?-1===i(e.primaryVersion,d[e.name].primaryVersion)&&(d[e.name]=e):d[e.name]=e}));var s=Object.keys(d).filter((function(e){return o.some((function(n){return n.name===d[e].name}))})).map((function(e){return d[e]}));if(!s.length)return!1;var c=function(e){return o.some((function(n){return n.name===e.name&&-1!==i(n.primaryVersion,e.primaryVersion)}))};return(null==r?void 0:r.isStrict)?s.every((function(e){return c(e)})):s.some((function(e){return c(e)}))}(navigator.userAgent,n);if(!o){var a=document.createElement("style");document.head.appendChild(a);var d="anim-obsolete-close ".concat(500,"ms forwards ease-in-out"),s="anim-obsolete-close {to { transform: translateY(-100%) }}";a.textContent="[data-obsolete-init] {\ncolor: #fff;\nbackground: red;\nposition: fixed;\nwidth: 100vw;\npadding: 4px;\nwhite-space: pre;\ntop: 0; left: 0;\ntext-align: center;\nz-index: 9999;\n}\n[data-obsolete-init] a {\n  text-decoration: underline;\n  cursor: pointer;\n}\n[data-obsolete-close] {\n-webkit-animation: ".concat(d,";\n-moz-animation: ").concat(d,";\nanimation: ").concat(d,";\n}\n@-webkit-keyframes ").concat(s,"\n@-moz-keyframes ").concat(s,"\n@keyframes ").concat(s);var c=document.createElement("div");c.setAttribute("data-obsolete-init","");var u=!1;if(c.addEventListener("mousemove",(function(){return u=!0}),{passive:!0}),c.addEventListener("click",(function(e){if(u&&(u=!1),e.target instanceof HTMLAnchorElement){if(!e.target.hasAttribute("href")){var t=window.open();t.document.write(n.join("\n")),t.document.body.style.whiteSpace="pre",t.document.body.style.display="flex",t.document.body.style.justifyContent="center",t.document.close()}}else c.setAttribute("data-obsolete-close",""),setTimeout((function(){c.parentElement.removeChild(c),document.head.removeChild(a)}),500)}),{passive:!0}),r.template)c.innerHTML=r.template.replace("{{browsers}}",n.join("\n"));else c.appendChild(document.createElement("div")).textContent="Your browser is not supported",c.appendChild(document.createElement("a")).textContent="Show supported browsers";document.body?document.body.appendChild(c):window.addEventListener("load",(function(){return document.body.appendChild(c)}),{once:!0,passive:!0}),console&&console.error("Not supported browser",{supportedBrowsers:n,userAgent:window.navigator&&window.navigator.userAgent})}return o};(function() {
  'use strict';
  obsolete([
    'chrome 120',
    'chrome 119',
    'chrome 118',
    'chrome 117',
    'chrome 116',
    'chrome 115',
    'chrome 114',
    'chrome 113',
    'chrome 112',
    'chrome 111',
    'edge 120',
    'edge 119',
    'firefox 121',
    'firefox 120',
    'opera 104',
    'opera 103',
    'safari 17.2',
    'safari 17.1',
    'safari 17.0'
  ],{
    name: 'obsolete',
    isStrict: true
  });
})();
