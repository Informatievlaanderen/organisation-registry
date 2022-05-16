<template>
  <header id="vlaanderen-top">
    <div id="vlaanderen-navigation">
      <a
        id="vlaanderen-link"
        href="https://www.vlaanderen.be/nl"
        target="_self"
      >
        <div id="vlaanderen-top-logo"></div>
        <span>Vlaanderen</span>
      </a>
    </div>
  </header>
</template>

<style scoped>
#vlaanderen-top {
  width: 100%;
  min-width: 768px;
  background: #fff;
  color: #333;
  border-bottom: 1px solid #cbd2da;
  font-size: 16px;
  font-family: flanders-sans;
  -webkit-text-size-adjust: none;
  font-smoothing: antialiased !important;
  text-rendering: optimizeLegibility;
}

#vlaanderen-navigation {
  white-space: nowrap;
}

#vlaanderen-link {
  display: inline-block;
  font-family: flanders-sans;
  font-weight: 500;
  text-decoration: none;
  outline: none;

  color: #333332;
  font-size: 1.1875em;
  line-height: 2.31579em;
  text-transform: none;
  transition: color 0.2s;
}

#vlaanderen-link:hover {
  color: #06c;
}

#vlaanderen-top-logo {
  position: relative;
  width: 47px;
  background-color: #fff;
  overflow: hidden;
  transition: background-color 0.2s;
  background-image: url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABEAAAAbCAYAAACa9mScAAAEk0lEQVR42pxV228UVRyendnt3mb2fpm9dtvdtrDixmrSIN4SNfiEJSXBB9MQkWBMJKjhBR8Mf4DwpA8kKglqKBgSrSUkEpsgSIwPoC3strSL3V7YS7d7m73MXmbX76zsppqK6CRnZs6cc77f9/t+l6GoTZfT4XSFHg8dpf7jRbdvNE3ZrLYnfL2+C61ma/h/gfT19u0Mbg/+qFAodtUb9e9kMhlhFWAYhn4UEAYHaZfTdVqtVg/F4/HD+UL+psflOaJRaxx6vb5/I7Nx+99A5AaDQWc0Gg2JRGLvYnTxEjS5iMPPLcWWXne73Edjy7HzTt7Zp9PpDjZxiVUxsnZ/bapcLgtdJj09PQq4MBu9F/1hcHDwpMViOZjL5T5eXVv9jOf548Vi8deKWIl53d5TYMuxWvYF3s7vzGQzU/V6vdkGEUWxJjWkNWhyxmQyvV2tVq+kUql3GlKjoexRvgJWLrD6RigK02az2Qa9ygzN7F9Pr18Eq/W2O7DG+by+L8BotNFonIvMRd7E+4jX43VIknQFlt/SarUMLEdu/XbrODTsGwgMMDJapu9GZ6B/4FO5XD6KA+fvhO+MVyoVcWho6BTmnkQycR2HPLDsJpuhA5XP53+HoQ8EQegKTjdbzf04EA1Hwodz+ZwU8AdOyhn5k4jUVbi6gD0Nu80+sjkapVLpPlh3haVBfQXiXc7msoXgtuAbVqv1PegyCd9/wbpQq9WWVSrV7ofmCRS+ipFUyBVNn8/3bavVKs3Nz+11Op1P1eo1CmABpMEINDmNfVtnLGJ+c2V15ZLX6z0CABasPsGBGCWjXuVt/MsA+Qm69EMX/h+Tjdw4jjOwLLsPryIAvyRpj4PDVbFaSqaSE3CRVSqV2yiBipP9/n7/bpTEM8lk8hymxnZtgIETDzfozkH1eaS8WcfpQvgObcW7WMugFMZJoT6IUtHj9nyI7J4J7Qjd6BQYQ6wXCoV5JBDl4B3PA8AAUaOYV4WCMMnpuHGL2RIkm8HuxvLK8gFENd6iWn9WMQAqGE0MkcwRjT3kmVpPRbCRQraeJa67ne5jyCkKJUQtLC6cRQCGsX6tDYKYL8GVMKz3qlVqGfR5Gu8bAF0j6xCfRPB7lmPH4CrbERQByOSF/HQbpFQuNYql4gQOsbCkhag2gKSwlG1rUCk3UeHHyDraxqGONiajya7n9HvkHVRQn7GarTtwmMGgH0SOjHZypDfSs0j5KZvN9hHcDUlNqcax3D5EydLtXBA1DNonYI2IXCMtF2D2zjrRAS79TIzApVEiJRrYGTStSfmmeohiUKRiodFduPUs+u6LBaHweTe9GSYHIZOR+cgu6HGv+/3v2QdrLZVSVUcCjqEJPQbgC2hKJbKG9B+sS/U4ut3Xf6mdrdIY1Xwbonk0Gs1LaFTD6XR6AuxAQuKsFisPba5vrqMtQeB3C/5eRlHSsH4A0WKy2ew0ki/psDvGUWcnUP0VgNbBuiJ76P9ERlN+v38Uv4/3Z2ZnXkO7SIChCq30K/ScMYA0UagLskf5r8AlB1hUUOE5Mkd0aLTPQ2jq7yJq2/8QYADHfmEQMqqC8gAAAABJRU5ErkJggg==");

  display: inline-block;
  float: left;
  margin-right: 5px;
  height: 44px;
  background-repeat: no-repeat;
  background-position: 11px center;
}

#vlaanderen-top-logo:after {
  content: "";
  display: block;
  position: absolute;
  left: 28px;
  top: -4px;
  height: 54px;
  width: 1px;
  background-image: none !important;
  background-color: #cbd2da;
  transform: rotate(-20deg);
  transform-origin: top left;

  /* eslint-disable-next-line */
  background-image: url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAsCAYAAABsSzi3AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyhpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMTExIDc5LjE1ODMyNSwgMjAxNS8wOS8xMC0wMToxMDoyMCAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTUgKE1hY2ludG9zaCkiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6Q0UxRTYyRjlGRkYwMTFFNUI5NjNCN0I5MjREOTYwN0QiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6Q0UxRTYyRkFGRkYwMTFFNUI5NjNCN0I5MjREOTYwN0QiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDpDRTFFNjJGN0ZGRjAxMUU1Qjk2M0I3QjkyNEQ5NjA3RCIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDpDRTFFNjJGOEZGRjAxMUU1Qjk2M0I3QjkyNEQ5NjA3RCIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pif4ca8AAAD7SURBVHjapNZdCwFBGIbhaf//X3DmTMjH5uNgFVKIJCRxICnFgZCwMZ5XO9qyaXeet+7Tq3ZndmccrfVSE+MopVJIK8sRII3mDLBBDQaQybFAF80Y4IKqDCCTYYEB6jGAj8oMYFbDZ4ApajKATJEFCujEAGvkMYBMngUqaMcAe1RngNhf6D9AXuSKAc7IZQCzGpoBOmjMAI9gZ1oDZmvfGWCE+gzw+rcnnJhb3g2W1RpYoBYDyGRZoIYODLCN+kKTAJGPkRTwgtPcGjiiEgP8/C9tgHb4RmMD3MJb2wYwp9eTAeRGM2SA74WEAeTwuTLARG40DPB5jLcAAwA4ZXyTFWLRuQAAAABJRU5ErkJggg==");
}

@media (max-width: 580px) {
  #vlaanderen-top {
    display: none !important;
  }
}
</style>
