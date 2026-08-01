
mergeInto(LibraryManager.library, {

  Quit: function () {
    console.log("Quit window");
    window.close();
  },
  setCookie: function (cname, cvalue) {
       var d = new Date();
       d.setTime(d.getTime() + (10*24*60*60*365));
       var expires = "expires="+ d.toUTCString();
       document.cookie = UTF8ToString(cname) + "=" + UTF8ToString(cvalue) + ";" + expires + ";path=/";
       console.log('set cookie='+document.cookie);
    },
 
    getCookie: function (cname) {
       var ret="";
       var name = UTF8ToString(cname) + "=";
       var decodedCookie = decodeURIComponent(document.cookie);
       console.log('get cookie='+decodedCookie);
       var ca = decodedCookie.split(';');
       for(var i = 0; i <ca.length; i++) {
           var c = ca[i];
           while (c.charAt(0) == ' ') {
               c = c.substring(1);
           }
           if (c.indexOf(name) == 0) {
               ret=c.substring(name.length, c.length);
               break;
           }
       }
       var buffer = _malloc(lengthBytesUTF8(ret) + 1);
       stringToUTF8(ret, buffer, ret.length + 1);
       return buffer;
    },
});