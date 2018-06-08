$(document).ready(function () {
    var $filter = $('.student-submissions__filter');
    var $input = $filter.find('.student-submissions__filter__input');
    var $loadingIcon = $filter.find('.loading-icon');
    var urlTemplate = $filter.data('url');
    var hasActiveRequest = false;
    $input.keyup(function () {
        var $table = $('.student-submissions table');
        var filterContent = $input.val();
        var url = urlTemplate.replace('NAME', filterContent);
        
        hasActiveRequest = true;
        setTimeout(function () {
            if (hasActiveRequest) 
                $loadingIcon.show();
        }, 1000);        
        
        $.get(url).done(function (data) {
            $table.replaceWith($(data));
        }).fail(function (e) {
            console.log('Не удалось загрузить решения студентов');
            console.error(e);
            alert('Не удалось загрузить решения студентов. Попробуйте ещё раз');            
        }).always(function () {
            hasActiveRequest = false;
            $loadingIcon.hide();
        });
    });
    
    $('.student-submissions').on('click', 'table tr.has-submissions', function (e) {
        var $link = $(e.target).closest('tr').find('a');
        if ($link.length > 0) 
            window.location = $link.attr('href');
    });
});