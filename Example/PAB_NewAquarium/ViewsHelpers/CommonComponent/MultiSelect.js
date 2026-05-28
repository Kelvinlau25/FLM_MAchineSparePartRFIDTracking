$('.tr-select2').multiselect({
    buttonClass: 'form-select form-control text-start',
    templates: {
        button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>',
    }
});
$('.tr-select2-filter').multiselect({
    buttonClass: 'form-select form-control text-start',
    enableCaseInsensitiveFiltering: true,
    enableFiltering: true,
    templates: {
        button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>',
        filter: '<div class="multiselect-filter"><div class="input-group input-group-sm p-1"><div class="input-group-prepend"></div><input class="form-control multiselect-search" type="text" /></div></div>'
    }
});
$('.multiselectpicker').multiselect({
    buttonClass: 'form-select form-control text-start',
    templates: {
        button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>',
    }
});
$('.multiselectpicker-filter').multiselect({
    buttonClass: 'form-select form-control text-start',
    enableCaseInsensitiveFiltering: true,
    enableFiltering: true,
    templates: {
        button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>',
        filter: '<div class="multiselect-filter"><div class="input-group input-group-sm p-1"><div class="input-group-prepend"></div><input class="form-control multiselect-search" type="text" /></div></div>'
    }
});
$('.multiselect-native-select > .btn-group').addClass("w-100");
$('.multiselect-native-select > .btn-group > .multiselect').removeClass("text-center");
$('.multiselect-native-select > .btn-group > .multiselect-container').addClass("w-100");
$('.dropdown-toggle').click(function () {
    $(this).next().find('input.multiselect-search').focus();
});