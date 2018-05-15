var sys = (function () {

    var 
        CheckMail, blockaccented,
        formatNumber, formatMoney, blockword,
        Alert, ConfirmDialog, CallAjax, CallAjaxPost, CallAjaxPostAsync,
        Paging, EncryptS, DecryptS, Loading,
        HideLoading, disButon, enButon, OnTop,
        GetDatetime, dateCurrent, formatDateTime, formatDateTimeSQL,
        getPostOffice, checkNull, resetForm;

    //alert dialog
    Alert = function (type, text) {
        var myStack = { "dir1": "down", "dir2": "right", "push": "top" };
        var opts = {
            title: "Over Here",
            text: "Check me out. I'm in a different stack.",
            stack: myStack,
        };
        switch (type) {
            case 'error':
                opts.title = "Oh No";
                opts.text = text == null ? "Thao tác thất bại!" : text;
                opts.type = "error";
                break;
            case 'info':
                opts.title = "Breaking News";
                opts.text = "Have you met Ted?";
                opts.type = "info";
                break;
            case 'success':
                opts.title = "Thông báo";
                opts.text = text == null ? "Thao tác thành công!" : text;
                opts.type = "success";
                break;
        }
        new PNotify(opts);
    };

    //confirm dialog
    ConfirmDialog = function (callback) {
        
        (new PNotify({
            title: 'Chú ý',
            text: 'Bạn chắc chắn muốn thực hiện thao tác này?',
            hide: false,
            confirm: {
                confirm: true
            },
            buttons: {
                closer: false,
                sticker: false
            },
            history: {
                history: false
            }
        })).get().on('pnotify.confirm', function () {
            if (callback && typeof callback === 'function') {
                callback();
                return;
            };
        }).on('pnotify.cancel', function () {
        });
    };

    //checkmail
    CheckMail = function (email) {
        var re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
        return re.test(email);
    };

    // chặn chử có dấu
    blockaccented = function (object) {
        object.keyup(function() {
            var check = /[^a-zA-Z0-9_-]/g;
            if (check.test(object.val())) {
                object.val(object.val().replace(/[^a-zA-Z0-9_-]/g, ''));
            }
        });
    };

    //chặn nhập chử
    blockword = function (object) {
        object.keyup(function() {
            var check = /[^0-9.]/;
            if (check.test(object.val())) {
                object.val(object.val().replace(/[^0-9.]/, ''));
            }
        });
    };

    //call ajax to get data
    CallAjax = function (url, params) {
        return $.ajax({
            Type: "GET",
            url: url,
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });
    };
    //call ajax to get data
    CallAjaxPost = function (url, params) {
        return $.ajax({
            url: url,
            type: "POST",
            data: JSON.stringify(params),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });
    };

    //call ajax to get data
    CallAjaxPostAsync = function (url, params) {
        return $.ajax({
            type: "POST",
            url: url,
            data: params,
            dataType: 'json',
            contentType: false,
            processData: false,
            async: false
        });
    };
    //formatMoney
    formatMoney = function (num, fix) {
        if (num === 0) return num;

        var p = num.toFixed(fix);

        if (p < 500) return (p + "").replace(".", ",");

        return p.split("").reverse().reduce(function (acc, num, i, orig) {
            return num + (i && !(i % 3) ? "." : "") + acc;
        }, "");
    };

    //format number
    formatNumber = function (obj) {
        obj = $(obj);
        var fix = 0;

        if (obj.attr('fix'))
            fix = parseInt(obj.attr('fix'));

        obj.keyup(function () {
            var v = obj.val();

            if (v.length === 0) return false;

            if (v.indexOf(',') > 0) return;

            v = v.replaceAll('.', '').replaceAll(',', '.');

            obj.val(formatMoney(parseFloat(v), fix));
        });
    };

    formatDateTime = function (_date, format) {
        if (_date !== null) {
            var date = _date;
            var subStrDate = date.substring(6);
            var parseIntDate = parseInt(subStrDate);
            date = new Date(parseIntDate);

            var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                nummonths = ['01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12']
            getPaddedComp = function(comp) {
                    return ((parseInt(comp) < 10) ? ('0' + comp) : comp)
                },
                formattedDate = format,
                o = {
                    "y+": date.getFullYear(), // year
                    "M+": months[date.getMonth()], //month text
                    "k+": nummonths[date.getMonth()], //month number
                    "d+": getPaddedComp(date.getDate()), //day
                    "h+": getPaddedComp((date.getHours() > 12) ? date.getHours() % 12 : date.getHours()), //hour
                    "H+": getPaddedComp(date.getHours()), //hour
                    "m+": getPaddedComp(date.getMinutes()), //minute
                    "s+": getPaddedComp(date.getSeconds()), //second
                    "S+": getPaddedComp(date.getMilliseconds()), //millisecond,
                    "b+": (date.getHours() >= 12) ? 'PM' : 'AM'
                };

            for (var k in o) {
                if (new RegExp("(" + k + ")").test(format)) {
                    formattedDate = formattedDate.replace(RegExp.$1, o[k]);
                }
            }
            return formattedDate;
        } else
            return 'N/A';
    };

    //convert datetime to datetime
    formatDateTimeSQL = function (date) {
        if (date !== null) {
            var d = new Date(date.split("-").reverse().join("-"));
            var dd = d.getDate();
            var mm = d.getMonth() + 1;
            var yyyy = d.getFullYear();

            if (dd < 10) {
                dd = '0' + dd;
            }

            if (mm < 10) {
                mm = '0' + mm;
            }

            var datetime = yyyy + "-" + mm + "-" + dd;
            return datetime;
        } else
            return 'N/A';
    }

    //Get datetime theo dinh dang format
    GetDatetime = function () {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();

        if (dd < 10) {
            dd = '0' + dd;
        }

        if (mm < 10) {
            mm = '0' + mm;
        }

        today = yyyy + '-' + mm + '-' + dd;
        return today;
    }

    // phan trang table
    Paging = function (object, ajaxSource, columnDefs) {
        var defcolumnDefs = [];
        if (columnDefs != null) defcolumnDefs = $.extend(defcolumnDefs, columnDefs);
        return object.dataTable({
            "responsive:": true,
            "bServerSide": true,
            "sAjaxSource": ajaxSource,
            "bProcessing": true,
            "columnDefs": defcolumnDefs
        });
    };

    //ma hoa javascript
    EncryptS = function (text) {
        var key = CryptoJS.enc.Utf8.parse('8080808080808080');
        var iv = CryptoJS.enc.Utf8.parse('8080808080808080');

        var x = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(text), key,
        {
            keySize: 128 / 8,
            iv: iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });

        $(".CryptoJS").val(x); // class .CryptoJS duoc khai bao o trang layout
        var Contents = $(".CryptoJS").val();

        //clear gia tri khi get xong
        $(".CryptoJS").val("");
        return Contents; //tra ve du lieu da ma hoa.
    };

    // giai ma javascipt
    DecryptS = function (text) {
        //return CryptoJS.AES.decrypt(text, "My Secret Passphrase");
        var key = CryptoJS.enc.Utf8.parse('8080808080808080');
        return CryptoJS.AES.decrypt(text, key).toString(iv);
    };

    //loading trang
    Loading = function () {
        $("#spinner-system").css("display", "block");
    };

    //hide load trang
    HideLoading = function () {
        $("#spinner-system").fadeOut('slow');
        $("#spinner-system").css("display", "none");
    };

    //vo hieu hoa button
    disButon = function (object) {
        $(object).attr('disabled', 'disabled');
    };

    //tat vo hieu hoa
    enButon = function (object) {
        $(object).removeAttr('disabled');
    };

    //len top
    OnTop = function () {
        $("html, body").animate({ scrollTop: 0 }, 'slow');
    };

    dateCurrent = function() {
        var today = new Date(),
            hh = today.getHours(),
            mm = today.getMinutes(),
            ss = today.getMilliseconds();
        if (hh.length < 2)
            hh = "0" + hh;
        if (mm.length < 2)
            mm = "0" + mm;
        return hh + ":" + mm + ":" + ss;
    }

    getPostOffice = function (options) {
        var defOptions = {
            querySelector: '',
            type: 0
        };
        if (options !== null) defOptions = $.extend(defOptions, options);
        if (defOptions.querySelector == '') return false;
        $.when(sys.CallAjax("/PostOffice/GetListPostOffice", { type: defOptions.type })).done(function (data) {
            if (data !== null) {
                var html = "<option value='0'>CHỌN TẤT CẢ</option>";
                $.each(data, function (key, value) {
                    html += "<option value='" + value.PostOfficeID + "'>" + value.POName + "</option>";
                });
                $(defOptions.querySelector).html(html);
            }
        });
    };

    //check null
    checkNull = function (value) {
        if (value === null || !value || (typeof value === 'undefined'))
            return false;
        return true;
    }
    //check null
    resetForm = function (value) {
        $(value + " input[name=Id]").val("0");
        $(value)[0].reset();
    }

    //return ket qua
    return {
        CheckMail: CheckMail,
        blockaccented: blockaccented,
        formatNumber: formatNumber,
        blockword: blockword,
        Alert: Alert,
        ConfirmDialog: ConfirmDialog,
        formatDateTime: formatDateTime,
        CallAjax: CallAjax,
        CallAjaxPost: CallAjaxPost,
        CallAjaxPostAsync: CallAjaxPostAsync,
        Paging: Paging,
        EncryptS: EncryptS,
        DecryptS: DecryptS,
        Loading: Loading,
        HideLoading: HideLoading,
        disButon: disButon,
        enButon: enButon,
        OnTop: OnTop,
        formatDateTimeSQL: formatDateTimeSQL,
        GetDatetime: GetDatetime,
        dateCurrent: dateCurrent,
        checkNull: checkNull,
        resetForm : resetForm
    };
})();

// CSRF (XSRF) security
function addAntiForgeryToken(data) {
    //if the object is undefined, create a new one.
    if (!data) {
        data = {};
    }
    //add token
    var tokenInput = $('input[name=__RequestVerificationToken]');
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
};

//bin run
$(window).bind("load", function () {

});
