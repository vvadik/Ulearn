$(document).ready(function () {
	var $courseStatistics = $('.course-statistics');

	var addColspan = function($td, diff) {
		var colspan = parseInt($td.attr('colspan'));
		$td.attr('colspan', colspan + diff);
	}

	var toggleUnitScoringGroup = function ($scoringGroupTitle) {
		var unitId = $scoringGroupTitle.data('unitId');
		var scoringGroupId = $scoringGroupTitle.data('scoringGroup');
		var isExpanded = !$scoringGroupTitle.data('expanded');
		$scoringGroupTitle.data('expanded', isExpanded);

		var $expandIcon = $scoringGroupTitle.find('.expand-icon');
		$expandIcon.text(isExpanded ? '◀' : '▶');
		$expandIcon.remove();
		if (isExpanded)
			$scoringGroupTitle.find('a').prepend($expandIcon);
		else
			$scoringGroupTitle.find('a').append($expandIcon);

		var filterByUnitAndScoringGroup = '[data-unit-id="' + unitId + '"][data-scoring-group="' + scoringGroupId + '"]';
		$courseStatistics.find('.slide-score' + filterByUnitAndScoringGroup).toggle(isExpanded);
		$courseStatistics.find('.slide-title' + filterByUnitAndScoringGroup).toggle(isExpanded);
		$courseStatistics.find('.slide-max-score' + filterByUnitAndScoringGroup).toggle(isExpanded);
		var slidesCount = $courseStatistics.find('.slide-title' + filterByUnitAndScoringGroup).length;

		var $unitTitleTh = $courseStatistics.find('.unit-title[data-unit-id="' + unitId + '"]').closest('th');
		addColspan($unitTitleTh, isExpanded ? slidesCount : -slidesCount);
		
		$courseStatistics.find(filterByUnitAndScoringGroup).toggleClass('in-expanded-scoring-group', isExpanded);
		$courseStatistics.toggleClass('with-expanded-scoring-group', $courseStatistics.find('.in-expanded-scoring-group').length > 0);
	}

	$courseStatistics.find('.expand-scoring-group__link').click(function(e) {
		e.preventDefault();
		var $parent = $(this).closest('.scoring-group-title');
		toggleUnitScoringGroup($parent);
	});

	$('.course-statistics__enable-scoring-group__checkbox').change(function() {
		var isChecked = $(this).prop('checked');
		var scoringGroupId = $(this).data('scoringGroup');
		
		$('.scoring-group-title[data-scoring-group="' + scoringGroupId + '"]').each(function () {
			/* Collapse if expanded */
			if ($(this).data('expanded'))
				toggleUnitScoringGroup($(this));

			var unitId = $(this).data('unitId');
			var $unitTitleTh = $('.unit-title[data-unit-id="' + unitId + '"]').closest('th');
			addColspan($unitTitleTh, isChecked ? 1 : -1);
		});

		var filterByScoringGroup = '[data-scoring-group="' + scoringGroupId + '"]';
		$courseStatistics.find('.scoring-group-title' + filterByScoringGroup).toggle(isChecked);
		$courseStatistics.find('.scoring-group-score' + filterByScoringGroup).toggle(isChecked);
		$courseStatistics.find('.scoring-group-max-score' + filterByScoringGroup).toggle(isChecked);

		var countChecked = $('.course-statistics__enable-scoring-group__checkbox:checked').length;
		if (countChecked <= 1)
			$('.course-statistics__enable-scoring-group__checkbox:checked').attr('disabled', 'disabled');
		else
			$('.course-statistics__enable-scoring-group__checkbox').removeAttr('disabled');
	});

	var compareByKeyFunction = function ($first, $second, keyFunction, order) {
		var firstValue = keyFunction($first);
		var secondValue = keyFunction($second);
		if (firstValue < secondValue)
			return order === 'asc' ? -1 : 1;
		if (firstValue > secondValue)
			return order === 'asc' ? 1 : -1;
		return 0;
	}

	/* Table sorting */
	var sortTable = function ($table, sortingFunctions, groupingFunction) {
		$table.find('tbody .group-header-row').remove();
		var $rows = $table.find('tbody tr').get();

		$rows.sort(function (first, second) {
			var $firstRow = $(first);
			var $secondRow = $(second);
			var compare;
			if (groupingFunction) {
				compare = compareByKeyFunction($firstRow, $secondRow, groupingFunction);
				if (compare !== 0)
					return compare;
			}
			for (var idx in sortingFunctions) {
				if (sortingFunctions.hasOwnProperty(idx)) {
					var sortingFunction = sortingFunctions[idx];
					compare = sortingFunction($firstRow, $secondRow);
					if (compare !== 0)
						return compare;
				}
			}
			return 0;
		});

		var $tbody = $table.children('tbody');
		var prevGroupingValue = undefined;
		$.each($rows, function (index, row) {
			if (groupingFunction) {
				var currentGroupingValue = groupingFunction($(row));
				if (prevGroupingValue !== currentGroupingValue) {
					var $groupHeader = $('<tr class="group-header-row"><td colspan="1000" class="group-header"></td></tr>');
					$groupHeader.find('td').text(currentGroupingValue ? currentGroupingValue : 'Вне групп');
					$tbody.append($groupHeader);
				}
			}
			$tbody.append(row);
		});
	}

	var getCompareFunction = function(filter, order) {
		return function ($firstRow, $secondRow) {
			var firstValue = $firstRow.children('td' + filter).text().toUpperCase();
			var secondValue = $secondRow.children('td' + filter).text().toUpperCase();

			if (firstValue < secondValue)
				return order === 'asc' ? -1 : 1;
			if (firstValue > secondValue)
				return order === 'asc' ? 1 : -1;
			return 0;
		}
	}

	var changeOrder = function(order) {
		return order === 'desc' ? 'asc' : 'desc';
	}

	var getSortingFunction = function($th, order) {
		var unitId = $th.data('unitId');
		var scoringGroupId = $th.data('scoringGroup');
		var slideId = $th.data('slideId');
		var filter = '[data-scoring-group="' + scoringGroupId + '"][data-unit-id="' + unitId + '"]';
		if (slideId)
			filter += '[data-slide-id="' + slideId + '"]';

		return getCompareFunction(filter, order);
	}

	var getCurrentSortingFunctions = function() {
		var $oldSortingThs = $courseStatistics.find('thead th[data-sorter="true"].sorted');
		$oldSortingThs.sort(function (first, second) {
			var firstValue = $(first).data('sorting-index');
			var secondValue = $(second).data('sorting-index');
			if (firstValue < secondValue) return -1;
			if (firstValue > secondValue) return 1;
			return 0;
		});
		return $.map($oldSortingThs, function (el) { return getSortingFunction($(el), $(el).data('order')); });
	}

	var groupingFunction = function ($row) { return $row.data('groups'); };

	$('#grouping-by-group').change(function() {
		var isGrouppingEnabled = $(this).is(':checked');
		var sortingFunctions = getCurrentSortingFunctions();
		sortTable($courseStatistics, sortingFunctions, isGrouppingEnabled ? groupingFunction : undefined);
	});
	
	$courseStatistics.find('thead th[data-sorter="true"]').click(function (e) {
		e.preventDefault();
		window.getSelection().removeAllRanges();

		var $self = $(this);
		/* Exclude current column from sorting */
		$self.removeClass('sorted sorted-asc sorted-desc');

		var isGrouppingEnabled = $('#grouping-by-group').is(':checked');

		var sortingFunctions = [];
		if (e.shiftKey) {
			sortingFunctions = getCurrentSortingFunctions();
		} else {
			$courseStatistics.find('thead th[data-sorter="true"].sorted').data('order', undefined).removeClass('sorted sorted-asc sorted-desc');
		}
		/* Find maximum of sorting index of current sorting columns */
		var maxSortingIndex = 0;
		$courseStatistics.find('thead th[data-sorter="true"].sorted').each(function() {
			if ($(this).data('sorting-index') > maxSortingIndex)
				maxSortingIndex = $(this).data('sorting-index');
		});

		var order = changeOrder($self.data('order'));
		sortingFunctions.push(getSortingFunction($self, order));
		$self.data('order', order).data('sorting-index', maxSortingIndex + 1).addClass('sorted sorted-' + order);
		
		sortTable($courseStatistics, sortingFunctions, isGrouppingEnabled ? groupingFunction : undefined);
	});
});