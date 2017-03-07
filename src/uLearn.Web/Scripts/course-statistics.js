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
	});
});