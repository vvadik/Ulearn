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

		var filterByUnitAndScoringGroup = '[data-unit-id="' + unitId + '"][data-scoring-group="' + scoringGroupId + '"]';
		$courseStatistics.find('.slide-score' + filterByUnitAndScoringGroup).toggle(isExpanded);
		$courseStatistics.find('.slide-title' + filterByUnitAndScoringGroup).toggle(isExpanded);
		$courseStatistics.find('.slide-max-score' + filterByUnitAndScoringGroup).toggle(isExpanded);
		var slidesCount = $courseStatistics.find('.slide-title' + filterByUnitAndScoringGroup).length;

		var $unitTitleTh = $courseStatistics.find('.unit-title[data-unit-id="' + unitId + '"]').closest('th');
		addColspan($unitTitleTh, isExpanded ? slidesCount : -slidesCount);
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
			if ($(this).data('isExpanded'))
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
	
	/*
	$courseStatistics.tablesorter({
		widgets: ['zebra', 'stickyHeaders', 'group', 'scroller'],
		sortInitialOrder: $courseStatistics.data('initial-order') || 'asc',

		selectorHeaders: 'thead th',

		cssAsc: "headerSortUp",
		cssDesc: "headerSortDown",
		cssHeader: "header",
		tableClass: 'tablesorter',

		widgetOptions: {
			columns: ["primary", "secondary", "tertiary"],
			stickyHeaders_offset: "#header",
			group_forceColumn: [1],
			group_enforceSort: false,
			//scroller_height : 300,
			scroller_fixedColumns : 1,
			scroller_rowHighlight : 'hover',
		},

		textAttribute: 'data-sort-value'
	});
	*/
});