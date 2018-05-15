$(document).ready(function () {
    //chan nhap chu
    sys.blockword($("#frmCashout input[name=Value]"));

    //custom column table
    var columnDefs = [{ "width": "200px", "targets": 0 },
    {
        "targets": [6],
        render: function (data, type, row) {
            if (data == 1)
                return '<span class="label label-primary">Active</span>';
            if (data == 1)
                return '<span class="label label-primary">Active</span>';
            return '<span class="label label-default">Inactive</span>';
        }
    }];
    //render datatable
    var oTable = sys.Paging($('#tbCashout'), "/Cashout/AjaxHandler", columnDefs);



    //submit modal Category
    $('#frmCashout').submit(function () {
        if ($(this).valid()) {
            sys.Loading();
            var Id = $("#frmCashout input[name=Id]").val(),
            Value = $("#frmCashout input[name=Value]").val();
            var Flag = 1;
            $("#frmCashout input[name=Flag]").each(function () {
                if ($(this).is(':checked')) {
                    Flag = $(this).val();
                }
            });
            var obj = new Object();
            obj.Id = Id;
            obj.Value = Value;
            obj.Flag = Flag;

            $.when(sys.CallAjaxPost("/Cashout/Modify", obj)).done(function (data) {
                if (data.Status == "Success") {
                    $('#detail_modal').modal('toggle');
                    oTable.fnDraw();
                    sys.Alert("success");
                }
                else {
                    sys.Error("Lưu lại thất bại!");
                }
            });
            sys.HideLoading();
        }
        event.preventDefault();

    });

    

});

function getCashoutById(id) {
    sys.resetForm("#frmCashout");
    //kiem tra neu la update thi show thông tin, nguoc lai thi clear from
    if (id !== 0) {
        $.when(sys.CallAjaxPost("/Cashout/getCashoutById", { id: id })).done(function (data) {
            if (sys.checkNull(data)) {
                $("#frmCashout input[name=Id]").val(data.Id);
                $("#frmCashout input[name=Value]").val(data.Value);
                $("#frmCashout input[name=Flag]").parent().removeClass('checked');
                $("#frmCashout input[name=Flag]").removeAttr("checked");
                $("#frmCashout input[name=Flag][value=" + data.Flag + "]").parent().addClass('checked');
                $("#frmCashout input[name=Flag][value=" + data.Flag + "]").attr('checked', 'checked');
            }
        });
    }
}

function deleteCashoutById(id) {
    // disable
    sys.ConfirmDialog(function () {
        $.when(sys.CallAjaxPost("/Cashout/DeleteCashout", { id: id })).done(function (data) {
            if (data.Status == "Success") {
                $('#tbCashout').dataTable().fnDraw();
                sys.Alert("success");
            } else {
                sys.Alert("error");
            }
        });
    });
}
