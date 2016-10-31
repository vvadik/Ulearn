$(function() {
    $.fn.smoothScroll = function (callback) {
    	var $object = this;
    	if ($object.length) {
    		var scrollTo = function() {
			    $('body').animate({
					scrollTop: $object.offset().top - $('.navbar').height()
				},
				{
					duration: 1000,
					complete: callback
				});
    		}
    		if (!$object.is(':visible')) {
			    $object.removeClass('hidden').removeClass('hidden-xs').removeClass('hidden-xs-inline');
			    $object.fadeIn(400, 'linear', scrollTo);
		    } else
			    scrollTo();
		    return true;
        } 
        return false;
    }

    $('a[href*="#"]:not([href="#"])').click(function() {
        if (location.pathname.replace(/^\//,'') == this.pathname.replace(/^\//,'') && location.hostname == this.hostname) {
            var $target = $(this.hash);
            $target = $target.length ? $target : $('[name=' + this.hash.slice(1) +']');
            return ! $target.smoothScroll();
        }
    });
});