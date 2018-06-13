$(document).ready(function () {
    var $filter = $('.student-submissions__filter');
    var $input = $filter.find('.student-submissions__filter__input');
    var $loadingIcon = $filter.find('.loading-icon');
    var urlTemplate = $filter.data('url');
    var activeRequest = false;
    $input.keyup(function () {
        var $table = $('.student-submissions table');
        var filterContent = $input.val();
        var url = urlTemplate.replace('NAME', filterContent);
        
        setTimeout(function () {
            if (activeRequest !== false) {
                $loadingIcon.show();                
            }
        }, 1000);        
        
        if (activeRequest !== false) 
            activeRequest.abort();
        
        activeRequest = $.get(url).done(function (data) {
            $table.replaceWith($(data));
        }).fail(function (jqh, textStatus, e) {
            if (textStatus === 'abort')
                return;
            console.log('Не удалось загрузить решения студентов');
            console.error(e, textStatus);
            alert('Не удалось загрузить решения студентов. Попробуйте ещё раз');            
        }).always(function (_, textStatus) {
            if (textStatus !== 'abort') {
                activeRequest = false;
                $loadingIcon.hide();
            }
        });
    });
    
    $('.student-submissions').on('click', 'table tr.has-submissions', function (e) {
        var $link = $(e.target).closest('tr').find('a');
        if ($link.length > 0) 
            window.location = $link.attr('href');
    });
});