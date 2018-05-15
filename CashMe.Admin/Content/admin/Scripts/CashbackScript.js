$(document).ready(function () {

    //custom column table
    var columnDefs = [{ "width": "200px", "targets": 0 },{
        "targets": [0],
        orderable: false,
        render: function (data, type, row) {
            var html = '<div class="btn-group">';
            html += '<a href="#" role="button" onclick="javascript: getCashbackById(' + data + ');" data-toggle="modal" data-target="#detail_modal" class="btn btn-primary btn-embossed btn-sm"><i class="fa fa-edit"></i> Sửa</a>';
            html += '<a href="#" role="button" onclick="javascript: deleteCashbackById(' + data + ');"  class="btn btn-warning btn-embossed btn-sm"><i class="fa fa-remove"></i> Xóa</a>';
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
    var oTable = sys.Paging($('#tbCashback'), "/Cashback/AjaxHandler", columnDefs);



    //submit modal 
    $('#frmCashback').submit(function () {
        if ($(this).valid()) {
            sys.Loading();
            var Id = $("#frmCashback input[name=Id]").val();
            var Flag = 1;
            var Linked_SiteId = $("#frmCashback select[name=Linked_SiteId]").find("option:selected").val();
            var CategoriesId = $("#frmCashback select[name=CategoriesId]").find("option:selected").val();
            var PercentId = $("#frmCashback select[name=PercentId]").find("option:selected").val();
            $("#frmCashback input[name=Flag]").each(function () {
                if ($(this).is(':checked')) {
                    Flag = $(this).val();
                }
            });
            var obj = new Object();
            obj.Id = Id;
            obj.Linked_SiteId = Linked_SiteId;
            obj.CategoriesId = CategoriesId;
            obj.PercentId = PercentId;
            obj.Flag = Flag;

            $.when(sys.CallAjaxPost("/Cashback/Modify", obj)).done(function (data) {
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

function getCashbackById(id) {
    sys.resetForm("#frmCashback");
    //kiem tra neu la update thi show thông tin, nguoc lai thi clear from
    if (id !== 0) {
        $.when(sys.CallAjaxPost("/Cashback/getCashbackById", { id: id })).done(function (data) {
            if (sys.checkNull(data)) {
                $("#frmCashback input[name=Id]").val(data.Id);
                $("#frmCashback select[name=Linked_SiteId]").val(data.Linked_SiteId);
                $("#frmCashback select[name=Linked_SiteId]").change();
                $("#frmCashback select[name=CategoriesId]").val(data.CategoriesId);
                $("#frmCashback select[name=CategoriesId]").change();
                $("#frmCashback select[name=PercentId]").val(data.PercentId);
                $("#frmCashback select[name=PercentId]").change();
                $("#frmCashback input[name=Flag]").parent().removeClass('checked');
                $("#frmCashback input[name=Flag]").removeAttr("checked");
                $("#frmCashback input[name=Flag][value=" + data.Flag + "]").parent().addClass('checked');
                $("#frmCashback input[name=Flag][value=" + data.Flag + "]").attr('checked', 'checked');
            }
        });
    }
}

function deleteCashbackById(id) {
    // disable
    sys.ConfirmDialog(function () {
        $.when(sys.CallAjaxPost("/Cashback/DeleteCashback", { id: id })).done(function (data) {
            if (data.Status == "Success") {
                $('#tbCashback').dataTable().fnDraw();
                sys.Alert("success");
            } else {
                sys.Alert("error");
            }
        });
    });
}
