$(function() {
    $.fn.smoothScroll = function (callback, $link) {
        var $object = this;
        var needScroll =  $link.data('scroll') !== false;
    	if ($object.length) {
    	    var scrollTo = function () {
	            if (!needScroll)
	                return;
			    $('body').animate({
					scrollTop: $object.offset().top - $('.navbar').height()
				},
				{
					duration: 1000,
					complete: callback
				});
    		}
    		if ($link && $link.data('toggle')) {
    			if (!$object.is(':visible')) {
				    $object.data('originalClasses', $object.attr('class'));
				    $object.removeClass('hidden').removeClass('hidden-xs').removeClass('hidden-xs-inline');
				    $object.fadeIn(200, 'linear', scrollTo);
    			} else {
				    $object.attr('class', $object.data('originalClasses'));
			    }
		    }
    		if ($object.is(':visible'))
			    scrollTo();
		    return true;
        } 
        return false;
    }

    $('a[href*="#"]:not([href="#"]):not(.no-smooth-scrolling)').click(function() {
        if (location.pathname.replace(/^\//,'') == this.pathname.replace(/^\//,'') && location.hostname == this.hostname) {
            var $target = $(this.hash);
            $target = $target.length ? $target : $('[name=' + this.hash.slice(1) +']');
            return ! $target.smoothScroll(null, $(this));
        }
    });
});