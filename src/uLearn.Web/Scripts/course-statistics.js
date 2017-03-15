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

	/* Table sorting */
	var sortTable = function ($table, sortingFunctions) {
		var $rows = $table.find('tbody tr').get();

		$rows.sort(function (first, second) {
			var $firstRow = $(first);
			var $secondRow = $(second);
			for (var idx in sortingFunctions) {
				if (sortingFunctions.hasOwnProperty(idx)) {
					var sortingFunction = sortingFunctions[idx];
					var compare = sortingFunction($firstRow, $secondRow);
					if (compare !== 0)
						return compare;
				}
			}
			return 0;
		});

		$.each($rows, function (index, row) {
			$table.children('tbody').append(row);
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
	
	$courseStatistics.find('thead th[data-sorter="true"]').click(function (e) {
		e.preventDefault();
		window.getSelection().removeAllRanges();

		var $self = $(this);
		/* Exclude current column from sorting */
		$self.removeClass('sorted sorted-asc sorted-desc');

		var sortingFunctions = [];
		if (e.shiftKey) {
			var $oldSortingThs = $courseStatistics.find('thead th[data-sorter="true"].sorted');
			$oldSortingThs.sort(function(first, second) {
				var firstValue = $(first).data('sorting-index');
				var secondValue = $(second).data('sorting-index');
				if (firstValue < secondValue) return -1;
				if (firstValue > secondValue) return 1;
				return 0;
			});
			sortingFunctions = $.map($oldSortingThs, function(el) { return getSortingFunction($(el), $(el).data('order')); });
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

		sortTable($courseStatistics, sortingFunctions);
	});
});