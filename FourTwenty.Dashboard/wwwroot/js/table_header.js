$(function () {
    $(document).on('change', '.dropdown-menu.filter-dropdown > .select-all', function (e) {
        var checkboxes = $(this).closest('.dropdown-menu').find(':checkbox');

        if ($(this).find(":checkbox").is(':checked')) {
            checkboxes.prop('checked', true);
        } else {
            checkboxes.prop('checked', false);
        }
        var funcName = $(this).closest('th').data('uniqename');
        window['applyFilters' + funcName](this);
    });
    $(document).on('change', '.dropdown-menu.filter-dropdown > .dropdown-item:not(.select-all)', function (e) {
        var allcheck = $(this).closest('.dropdown-menu').find(':checkbox:not(.first)');
        var first = $(this).closest('.dropdown-menu').find(':checkbox.first');
        var boolArray = allcheck.map(function (n) {
            return $(this).prop("checked");
        }).get();
        var res = boolArray.every(isTrue);

        if ($(this).find(":checkbox").is(':checked') && res) {
            first.prop('checked', true);
        } else {
            first.prop('checked', false);
        }
        var funcName = $(this).closest('th').data('uniqename');
        window['applyFilters' + funcName](this);
    });
    $(document).on('click', '.dropdown-menu.dropdown-menu-right', function (e) {
        e.stopPropagation();
    });
});

function getAllTableFilters(tableOrHead) {
    var jsObj = {};
    var items;
    if (tableOrHead.is('table'))
        items = tableOrHead.find('thead').find('.dropdown-menu');
    else if (tableOrHead.is('thead'))
        items = $(tableOrHead).find('.dropdown-menu');
    else
        return jsObj;
    items.map(function () {
        jsObj[$(this).data('property')] = $(this).find(':checkbox:checked:not(.first)').map(function () {
            return $(this).val();
        }).get();
    });
    return jsObj;
}

function checkAll(element) {
	$(element).closest("table").find("tbody").find("input[type=checkbox]").map(function() {
		if ($(element).is(":checked")) {
			$(this).prop("checked", true);
		} else {
			$(this).prop("checked", false);
		}

	});
}

function isTrue(element, index, array) {
    return element;
}