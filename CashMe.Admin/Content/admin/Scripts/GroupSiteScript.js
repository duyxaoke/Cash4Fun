$(document).ready(function () {

    //custom column table
    var columnDefs = [{ "width": "200px", "targets": 0 }, {
        "targets": [0],
        orderable: false,
        render: function (data, type, row) {
            var html = '<div class="btn-group">';
            html += '<a href="#" role="button" onclick="javascript: getGroupSiteById(' + data + ');" data-toggle="modal" data-target="#detail_modal" class="btn btn-primary btn-embossed btn-sm"><i class="fa fa-edit"></i> Sửa</a>';
            html += '<a href="#" role="button" onclick="javascript: deleteGroupSiteById(' + data + ');"  class="btn btn-warning btn-embossed btn-sm"><i class="fa fa-remove"></i> Xóa</a>';
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
    var oTable = sys.Paging($('#tbGroupSite'), "/GroupSite/AjaxHandler", columnDefs);



    //submit modal GroupSite
    $('#frmGroupSite').submit(function () {
        if ($(this).valid()) {
            sys.Loading();
            var Id = $("#frmGroupSite input[name=Id]").val(),
            GroupName = $("#frmGroupSite input[name=GroupName]").val();
            var Flag = 1;
            $("#frmGroupSite input[name=Flag]").each(function () {
                if ($(this).is(':checked')) {
                    Flag = $(this).val();
                }
            });
            var obj = new Object();
            obj.Id = Id;
            obj.GroupName = GroupName;
            obj.Flag = Flag;

            $.when(sys.CallAjaxPost("/GroupSite/Modify", obj)).done(function (data) {
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

function getGroupSiteById(id) {
    sys.resetForm("#frmGroupSite");
    //kiem tra neu la update thi show thông tin, nguoc lai thi clear from
    if (id !== 0) {
        $.when(sys.CallAjaxPost("/GroupSite/getGroupSiteById", { id: id })).done(function (data) {
            if (sys.checkNull(data)) {
                $("#frmGroupSite input[name=Id]").val(data.Id);
                $("#frmGroupSite input[name=GroupName]").val(data.GroupName);
                $("#frmGroupSite input[name=Flag]").parent().removeClass('checked');
                $("#frmGroupSite input[name=Flag]").removeAttr("checked");
                $("#frmGroupSite input[name=Flag][value=" + data.Flag + "]").parent().addClass('checked');
                $("#frmGroupSite input[name=Flag][value=" + data.Flag + "]").attr('checked', 'checked');
            }
        });
    }
}

function deleteGroupSiteById(id) {
    // disable
    sys.ConfirmDialog(function () {
        $.when(sys.CallAjaxPost("/GroupSite/DeleteGroupSite", { id: id })).done(function (data) {
            if (data.Status == "Success") {
                $('#tbGroupSite').dataTable().fnDraw();
                sys.Alert("success");
            } else {
                sys.Alert("error");
            }
        });
    });
}
