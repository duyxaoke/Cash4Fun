$(document).ready(function () {

    //custom column table
    var columnDefs = [{ "width": "200px", "targets": 0 }, {
        "targets": [0],
        orderable: false,
        render: function (data, type, row) {
            var html = '<div class="btn-group">';
            //html += '<a href="#" role="button" data-id=' + data + ' onclick="javascript: getRoleById(this);" data-toggle="modal" data-target="#detail_modal" class="btn btn-primary btn-embossed btn-sm"><i class="fa fa-edit"></i> Sửa</a>';
            //html += '<a href="#" role="button" onclick="javascript: deleteCategoriesById(' + data + ');"  class="btn btn-warning btn-embossed btn-sm"><i class="fa fa-remove"></i> Xóa</a>';
            html += '</div>';
            return html;
        }
    },
    {
        //"targets": [4],
        //render: function (data, type, row) {
        //    if (data == 1)
        //        return '<span class="label label-primary">Active</span>';
        //    return '<span class="label label-default">Inactive</span>';
        //}
    }];
    //render datatable
    var oTable = sys.Paging($('#tbRoles'), "/Role/AjaxHandler", columnDefs);

    //submit modal Category
    $('#frmRoles').submit(function () {
        if ($(this).valid()) {
            sys.Loading();
            var id = $("#frmRoles input[name=Id]").val(),
                role = $("#frmRoles input[name=RoleName]").val();
            //var Flag = 1;
            //$("#frmRoles input[name=Flag]").each(function () {
            //    if ($(this).is(':checked')) {
            //        Flag = $(this).val();
            //    }
            //});
            var obj = new Object();
            obj.Id = id;
            obj.RoleName = role;
            //obj.Flag = Flag;      
            $.when(sys.CallAjaxPost("/Role/Modify", obj)).done(function (data) {
                if (data.Status == "Success") {
                    $('#detail_modal').modal('toggle');
                    oTable.fnDraw();
                    sys.Alert("Thành công");
                }
                else {
                    sys.Error("Lưu thất bại!");
                }
            });
            sys.HideLoading();
        }
        event.preventDefault();
    });
});

function getRoleById(obj) {
    var id = $(obj).attr("data-id");
    sys.resetForm("#frmRoles");
    //kiem tra neu la update thi show thông tin, nguoc lai thi clear form
    if (id !== 0) {
        $.when(sys.CallAjaxPost("/Role/getRoleById", { id: id })).done(function (data) {
            if (sys.checkNull(data)) {
                $("#frmRoles input[name=Id]").val(data.Id);
                $("#frmRoles input[name=RoleName]").val(data.Name);
                //$("#frmRoles input[name=Flag]").parent().removeClass('checked');
                //$("#frmRoles input[name=Flag]").removeAttr("checked");
                //$("#frmRoles input[name=Flag][value=" + data.Flag + "]").parent().addClass('checked');
                //$("#frmRoles input[name=Flag][value=" + data.Flag + "]").attr('checked', 'checked');
            }s
        });        
    }
}

//function deleteCategoriesById(id) {
//    // disable
//    sys.ConfirmDialog(function () {
//        $.when(sys.CallAjaxPost("/Categories/DeleteCategories", { id: id })).done(function (data) {
//            if (data.Status == "Success") {
//                $('#tbCategories').dataTable().fnDraw();
//                sys.Alert("success");
//            } else {
//                sys.Alert("error");
//            }
//        });
//    });
//}
