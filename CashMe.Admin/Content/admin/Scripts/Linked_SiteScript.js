$(document).ready(function () {

    //custom column table
    var columnDefs = [{ "width": "200px", "targets": 0 }, {
        "targets": [0],
        orderable: false,
        render: function (data, type, row) {
            var html = '<div class="btn-group">';
            html += '<a href="#" role="button" onclick="javascript: getLinked_SiteById(' + data + ');" data-toggle="modal" data-target="#detail_modal" class="btn btn-primary btn-embossed btn-sm"><i class="fa fa-edit"></i> Sửa</a>';
            html += '<a href="#" role="button" onclick="javascript: deleteLinked_SiteById(' + data + ');"  class="btn btn-warning btn-embossed btn-sm"><i class="fa fa-remove"></i> Xóa</a>';
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
    var oTable = sys.Paging($('#tbLinked_Site'), "/Linked_Site/AjaxHandler", columnDefs);



    //submit modal Category
    $('#frmLinked_Site').submit(function () {
        if ($(this).valid()) {
            sys.Loading();
            var Id = $("#frmLinked_Site input[name=Id]").val(),
            GroupSiteId = $("#frmLinked_Site select[name=GroupSiteId]").find("option:selected").val();
            SiteName = $("#frmLinked_Site input[name=SiteName]").val(),
            LinkAffiliate = $("#frmLinked_Site input[name=LinkAffiliate]").val();
            var Flag = 1;
            $("#frmLinked_Site input[name=Flag]").each(function () {
                if ($(this).is(':checked')) {
                    Flag = $(this).val();
                }
            });
            var oldimage = $(".OldImage").val().trim();
            var LinkLogo = "";
            if ($("#Logo").val() !== "") {
                var formData = new FormData();
                var totalFiles = document.getElementById("Logo").files.length;
                var file = document.getElementById("Logo").files[0];
                formData.append("Logo", file);
                $.when(sys.CallAjaxPostAsync("/Linked_Site/UploadLogo", formData)).done(function (data) {
                    if(data.Status == "Success")
                        LinkLogo = data.Message;
                })
            };
            if (LinkLogo == "") {
                LinkLogo = oldimage;
            }
            var obj = new Object();
            obj.Id = Id;
            obj.GroupSiteId = GroupSiteId;
            obj.SiteName = SiteName;
            obj.LinkAffiliate = LinkAffiliate;
            obj.Logo = LinkLogo;
            obj.Flag = Flag;

            $.when(sys.CallAjaxPost("/Linked_Site/Modify", obj)).done(function (data) {
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

function getLinked_SiteById(id) {
    sys.resetForm("#frmLinked_Site");
    //kiem tra neu la update thi show thông tin, nguoc lai thi clear from
    if (id !== 0) {
        $.when(sys.CallAjaxPost("/Linked_Site/getLinked_SiteById", { id: id })).done(function (data) {
            if (sys.checkNull(data)) {
                $("#frmLinked_Site input[name=Id]").val(data.Id);
                $("#frmLinked_Site select[name=GroupSiteId]").val(data.GroupSiteId).change();
                //$("#frmLinked_Site select[name=GroupSiteId]").change();
                $("#frmLinked_Site input[name=SiteName]").val(data.SiteName);
                $("#frmLinked_Site input[name=LinkAffiliate]").val(data.LinkAffiliate);
                $("#frmLinked_Site input[name=Flag]").parent().removeClass('checked');
                $("#frmLinked_Site input[name=Flag]").removeAttr("checked");
                $("#frmLinked_Site input[name=Flag][value=" + data.Flag + "]").parent().addClass('checked');
                $("#frmLinked_Site input[name=Flag][value=" + data.Flag + "]").attr('checked', 'checked');
                $(".OldImage").val(data.Logo);
                $(".ReviewLogo").attr("src", data.Logo);

            }
        });
    }
}

function deleteLinked_SiteById(id) {
    // disable
    sys.ConfirmDialog(function () {
        $.when(sys.CallAjaxPost("/Linked_Site/DeleteLinked_Site", { id: id })).done(function (data) {
            if (data.Status == "Success") {
                $('#tbLinked_Site').dataTable().fnDraw();
                sys.Alert("success");
            } else {
                sys.Alert("error");
            }
        });
    });
}
