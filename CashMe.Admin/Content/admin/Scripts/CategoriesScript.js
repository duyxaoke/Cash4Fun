$(document).ready(function () {

    //custom column table
    var columnDefs = [{ "width": "200px", "targets": 0 }, {
        "targets": [0],
        orderable: false,
        render: function (data, type, row) {
            var html = '<div class="btn-group">';
            html += '<a href="#" role="button" onclick="javascript: getCategoriesById(' + data + ');" data-toggle="modal" data-target="#detail_modal" class="btn btn-primary btn-embossed btn-sm"><i class="fa fa-edit"></i> Sửa</a>';
            html += '<a href="#" role="button" onclick="javascript: deleteCategoriesById(' + data + ');"  class="btn btn-warning btn-embossed btn-sm"><i class="fa fa-remove"></i> Xóa</a>';
            html += '</div>';
            return html;
        }
    },
    {
        "targets": [4],
        render: function (data, type, row) {
            if (data == 1)
                return '<span class="label label-primary">Active</span>';
            return '<span class="label label-default">Inactive</span>';
        }
    }];
    //render datatable
    var oTable = sys.Paging($('#tbCategories'), "/Categories/AjaxHandler", columnDefs);



    //submit modal Category
    $('#frmCategories').submit(function () {
        if ($(this).valid()) {
            sys.Loading();
            var Id = $("#frmCategories input[name=Id]").val(),
            CategoriesName = $("#frmCategories input[name=CategoriesName]").val();
            var Flag = 1;
            $("#frmCategories input[name=Flag]").each(function () {
                if ($(this).is(':checked')) {
                    Flag = $(this).val();
                }
            });
            var obj = new Object();
            obj.Id = Id;
            obj.CategoriesName = CategoriesName;
            obj.Flag = Flag;

            $.when(sys.CallAjaxPost("/Categories/Modify", obj)).done(function (data) {
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

function getCategoriesById(id) {
    sys.resetForm("#frmCategories");
    //kiem tra neu la update thi show thông tin, nguoc lai thi clear from
    if (id !== 0) {
        $.when(sys.CallAjaxPost("/Categories/getCategoriesById", { id: id })).done(function (data) {
            if (sys.checkNull(data)) {
                $("#frmCategories input[name=Id]").val(data.Id);
                $("#frmCategories input[name=CategoriesName]").val(data.CategoriesName);
                $("#frmCategories input[name=Flag]").parent().removeClass('checked');
                $("#frmCategories input[name=Flag]").removeAttr("checked");
                $("#frmCategories input[name=Flag][value=" + data.Flag + "]").parent().addClass('checked');
                $("#frmCategories input[name=Flag][value=" + data.Flag + "]").attr('checked', 'checked');
            }
        });
    }
}

function deleteCategoriesById(id) {
    // disable
    sys.ConfirmDialog(function () {
        $.when(sys.CallAjaxPost("/Categories/DeleteCategories", { id: id })).done(function (data) {
            if (data.Status == "Success") {
                $('#tbCategories').dataTable().fnDraw();
                sys.Alert("success");
            } else {
                sys.Alert("error");
            }
        });
    });
}
