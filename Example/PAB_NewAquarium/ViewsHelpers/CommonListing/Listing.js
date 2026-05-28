var timeoutId;
var AIDetection = null;
var isAbort = false;

$('#btnReset').click(function () {
    $(':input').val('');
});

$(window).on('resize', adjustInputGroup);

$(document).on('click', function () {
    var dropdown = $(".overlay-dropdown-list");
    var dropdownMenu = $(".overlay-dropdown-list .dropdown-menu");
    dropdown.removeClass("show");
    dropdownMenu.removeClass("show");
});

function initPage() {
    if (performance.getEntriesByType("navigation")[0].type === "back_forward") {
        location.reload();
    }
}

function adjustInputGroup() {
    if (screen.width < 1920) {
        $('.responsive-input-group').addClass('input-group-sm');
    } else {
        $('.responsive-input-group').removeClass('input-group-sm');
    }
}

function calcDataTableHeight() {
    var winWidth = $(window).width();
    if (winWidth >= 1280 && winWidth <= 1599) {
        return $(window).height() * 43 / 100;
    } else {
        return $(window).height() * 50 / 100;
    }
};

function adjustDataTableColumn() {
    if (hasVerticalScrollbar()) {
        $('.dataTables_scrollHead').css('width', 'calc(100% - 3px)');
        $('.custom-toolbar').css('width', 'calc(100% - 3px)');
    }
    else {
        $('.dataTables_scrollHead').css('width', '100%');
        $('.custom-toolbar').css('width', '100%');
    }
}

function initDataTableComponent() {
    adjustInputGroup();
    $("div.go-to").html(`<div class="d-flex align-items-center p-0"><input class="form-control" type="text" id="txtGoTo" style="width:75px" value=1> &nbsp; <div class="page-info">of ${table.page.info().pages}</div></div>`);
    $("#txtGoTo").on('keyup', function (e) {
        try {
            var page = e.target.value;
            if ($.isNumeric(page) && page > 0 && page <= table.page.info().pages) {
                table.page(page - 1).draw('page');
            } else if (e.target.value.trim() == '') {

            } else {
                e.target.value = table.page() + 1;
            }

        } catch (ex) {
            e.target.value = 1;
        }
    });
    $("#txtGoTo").on('blur', function (e) {
        try {
            var page = e.target.value;
            if (page.trim() == '') {
                $("#txtGoTo").val(1);
                table.page(0).draw('page');
                $("#txtGoTo").focus();
            }
        } catch (ex) {
            e.target.value = 1;
        }
    });
    $(".dataTables_length .form-select").on('change', function () {
        $("#txtGoTo").val(table.page() + 1);
        $("div.page-info").html(`of ${table.page.info().pages}</div>`);
    });
    $('#tr-datatable').on('length.dt', function (e, settings, len) {
        var customToolbarDiv = $('.custom-toolbar');

        if (len == 10) {
            customToolbarDiv.removeClass('custom-toolbar-add-margin');
        } else {
            customToolbarDiv.addClass('custom-toolbar-add-margin');
        }
    });
    $('.dataTables_length select').addClass('tr-select2');
    $('.tr-select2').multiselect({
        buttonClass: 'form-select form-control text-start',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>',
        }
    });
    $('.tr-select').select2({
        theme: "bootstrap-5",
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        placeholder: $(this).data('placeholder'),
        minimumResultsForSearch: Infinity,
        templateSelection: function formatState(state) {
            return 'Status : ' + state.text;
        },
        dropdownCssClass: 'tr-select2-container',
        selectionCssClass: 'tr-no-border'
    });

    $(document).on('keyup', '#txtSearch', function (e) {

        table.search(e.target.value).draw();

        clearTimeout(timeoutId);

        timeoutId = setTimeout(() => {

            var dropdown = document.querySelector(".overlay-dropdown-list");

            dropdown.classList.add("show");
            var textValue = e.target.value;
            var encodeMessage = encodeText(textValue);

            if ($('#hdnAiIndicator').val() == "0") {

                if (textValue == encodeMessage) {
                    updateDropdownMenu(textValue);
                }
                else {
                    const dropdownMenu = document.querySelector('.tr-ddm.dropdown-menu.dropdown-menu-list');
                    dropdownMenu.innerHTML = '';
                    const aTag = document.createElement('a');
                    aTag.className = 'dropdown-item dropdown-menu-list-word-detect dropdown-item-word-detect no-redirect fw-bolder h-auto';
                    aTag.href = "#";
                    aTag.setAttribute('role', 'button');
                    const highlightedText = `<span class="text-wrap"> We couldn't find a match for <span style="color:red">"AI search function"</span>. Please try another search.</span>`;
                    aTag.innerHTML = highlightedText;
                    dropdownMenu.appendChild(aTag);
                    dropdownMenu.classList.add("show");
                }
            }

            table.search($('#txtSearch').val()).draw();
            $("div.page-info").html('of ' + table.page.info().pages + '</div>');
        }, 1000);
    });

    table.on('select deselect', function () {
        var rowCnt = table.rows({ selected: true }).count();
        if (rowCnt > 0) {
            $(".tr-trash").removeClass('isHide');
        } else {
            $(".tr-trash").addClass('isHide');
        }
    });

    table.on('draw', function () {
        adjustDataTableColumn();
    });

    table.columns.adjust().draw();
}

function hasVerticalScrollbar() {
    var table = $('#tr-datatable').DataTable();
    var tableWrapper = table.table().container();
    var tableBody = tableWrapper.querySelector('.dataTables_scrollBody');
    return tableBody.scrollHeight > tableBody.clientHeight;
}

function encodeText(message) {
    // Ecode text
    return String(message)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

function wordDetectionResult(text) {
    $('#txtSearch').val(text)
    table.search(text).draw();
    $("div.page-info").html('of ' + table.page.info().pages + '</div>');
}

function updateDropdownMenu(textValue) {
    if (textValue !== "" && textValue !== null && textValue !== undefined) {

        const uniqueValues = new Set();

        const flatList = data.reduce((acc, obj) => {
            Object.values(obj).forEach(value => {
                if (!uniqueValues.has(value)) {
                    uniqueValues.add(value);
                    acc.push(value);
                }
            });
            return acc;
        }, []);

        var formData = {
            api_key: "",
            input: textValue,
            compareList: flatList
        }
        if (AIDetection)
        {
            AIDetection = null
        }

       AIDetection = $.ajax({
            type: "POST",
            url: "/AIApi/WordDetection",
            data: formData,
            success: function (data) {
                if (data != "Error") {
                    const dropdownMenu = document.querySelector('.tr-ddm.dropdown-menu.dropdown-menu-list');
                    dropdownMenu.innerHTML = '';

                    const array = data.similarityList;
                    const lastTen = array.slice(-10);
                    const reverseList = lastTen.reverse();
                    var isSuggest = false;

                    const links = reverseList.map(item => {
                        return { href: '#', text: item[0], score: item[1] };
                    });

                    const boldPattern = new RegExp(textValue, 'gi');

                    links.forEach(link => {
                        if (link.score > 0.5) {
                            const aTag = document.createElement('a');
                            aTag.className = 'dropdown-item dropdown-menu-list-word-detect dropdown-item-word-detect';
                            aTag.href = link.href;
                            aTag.textContent = link.text;
                            aTag.setAttribute('role', 'button');
                            const highlightedText = link.text.replace(boldPattern, match => `<span style="font-weight: bold; color:#057FE5;">${match}</span>`);
                            aTag.innerHTML = highlightedText;
                            aTag.addEventListener('click', function (event) {
                                event.preventDefault();
                                wordDetectionResult(link.text);
                            });
                            dropdownMenu.appendChild(aTag);
                            isSuggest = true;
                        }
                    });

                    if (isSuggest == false) {
                        const aTag = document.createElement('a');
                        aTag.className = 'dropdown-item dropdown-menu-list-word-detect dropdown-item-word-detect no-redirect fw-bolder h-auto';
                        aTag.href = "#";
                        aTag.setAttribute('role', 'button');
                        const highlightedText = `<span class="text-wrap"> We couldn't find a match for <span style="color:red">"AI search function"</span>. Please try another search.</span>`;
                        aTag.innerHTML = highlightedText;
                        dropdownMenu.appendChild(aTag);
                    }
                    const aTag = document.createElement('a');
                    aTag.className = 'dropdown-item text-end dropdown-menu-list-word-detect dropdown-item-word-detect no-redirect';
                    aTag.href = "#";
                    aTag.setAttribute('role', 'button');
                    const highlightedText = `<span style="font-weight: bold; font-style: italic; font-size: 9px;">AI Powered By TMS </span>`;
                    aTag.innerHTML = highlightedText;
                    dropdownMenu.appendChild(aTag);
                    dropdownMenu.classList.add("show");
                }
                else {
                    $('#hdnAiIndicator').val("1");
                    $('#ai-unavailable-modal').modal('show');
                    $('.ai-image').hide();
                }

            },
           error: function (jqXHR, textStatus) {
               $('#hdnAiIndicator').val("1");
               if (textStatus !== 'abort')
                {
                    $('#ai-unavailable-modal').modal('show');
                    $('.ai-image').hide();
                }
            }
        });
    } else {
        var dropdown = document.querySelector(".overlay-dropdown-list");
        var dropdownMenu = document.querySelector(".overlay-dropdown-list .dropdown-menu");
        dropdown.classList.remove("show");
        dropdownMenu.classList.remove("show");
    }
}

window.addEventListener('beforeunload', () => {
    if (AIDetection) {
        AIDetection.abort();
        AIDetection = null;
    }
});

$(document).on("click", "a", function (e)
{
    if (AIDetection) {
        AIDetection.abort();
        AIDetection = null;
    }
})
