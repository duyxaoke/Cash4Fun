$(document).ready(function () {

    //custom column table
    var columnDefs = [
        { "width": "200px", "targets": 0 },{
        "targets": [0],
        orderable: false,
        render: function (data, type, row) {
            var html = '<div class="btn-group">';
            html += '<a href="#" role="button" onclick="javascript: getVoucherById(' + data + ');" data-toggle="modal" data-target="#detail_modal" class="btn btn-primary btn-embossed btn-sm"><i class="fa fa-edit"></i> Sửa</a>';
            html += '<a href="#" role="button" onclick="javascript: deleteVoucherById(' + data + ');"  class="btn btn-warning btn-embossed btn-sm"><i class="fa fa-remove"></i> Xóa</a>';
            html += '</div>';
            return html;
        }
    },
    {
        "targets": [5],
        render: function (data, type, row) {
            if (data == 1)
                return '<span class="label label-primary">Active</span>';
            return '<span class="label label-default">Inactive</span>';
        }
    }];
    //render datatable
    var oTable = sys.Paging($('#tbVoucher'), "/Voucher/AjaxHandler", columnDefs);



    //submit modal Category
    $('#frmVoucher').submit(function () {
        if ($(this).valid()) {
            sys.Loading();
            var Id = $("#frmVoucher input[name=Id]").val(),
            VoucherName = $("#frmVoucher input[name=VoucherName]").val(),
            Body = $("#frmVoucher textarea[name=Body]").val(),
            EndDate = $("#frmVoucher input[name=EndDate]").val();
            var Flag = 1;
            var Linked_SiteId = $("#frmVoucher select[name=Linked_SiteId]").find("option:selected").val();
            $("#frmVoucher input[name=Flag]").each(function () {
                if ($(this).is(':checked')) {
                    Flag = $(this).val();
                }
            });
            var obj = new Object();
            obj.Id = Id;
            obj.VoucherName = VoucherName;
            obj.Linked_SiteId = Linked_SiteId;
            obj.Body = Body;
            obj.EndDate = EndDate;
            obj.Flag = Flag;

            $.when(sys.CallAjaxPost("/Voucher/Modify", obj)).done(function (data) {
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

function getVoucherById(id) {
    sys.resetForm("#frmVoucher");
    //kiem tra neu la update thi show thông tin, nguoc lai thi clear from
    if (id !== 0) {
        $.when(sys.CallAjaxPost("/Voucher/getVoucherById", { id: id })).done(function (data) {
            if (sys.checkNull(data)) {
                $("#frmVoucher input[name=Id]").val(data.Id);
                $("#frmVoucher input[name=VoucherName]").val(data.VoucherName);
                $("#frmVoucher textarea[name=Body]").val(data.Body);
                //$("#frmVoucher input[name=EndDate]").datepicker('setDate', data.EndDate);
                $("#frmVoucher input[name=EndDate]").val(sys.formatDateTime(data.EndDate, 'y-k-d'));
                $("#frmVoucher select[name=Linked_SiteId]").val(data.Linked_SiteId);
                $("#frmVoucher select[name=Linked_SiteId]").change();
                $("#frmVoucher input[name=Flag]").parent().removeClass('checked');
                $("#frmVoucher input[name=Flag]").removeAttr("checked");
                $("#frmVoucher input[name=Flag][value=" + data.Flag + "]").parent().addClass('checked');
                $("#frmVoucher input[name=Flag][value=" + data.Flag + "]").attr('checked', 'checked');
            }
        });
    }
}

function deleteVoucherById(id) {
    // disable
    sys.ConfirmDialog(function () {
        $.when(sys.CallAjaxPost("/Voucher/DeleteVoucher", { id: id })).done(function (data) {
            if (data.Status == "Success") {
                $('#tbVoucher').dataTable().fnDraw();
                sys.Alert("success");
            } else {
                sys.Alert("error");
            }
        });
    });
}
