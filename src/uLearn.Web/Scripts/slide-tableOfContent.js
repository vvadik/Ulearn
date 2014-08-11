function hideTable() {
	if (!$(".side-bar").hasClass("hidden1")) {
		$(".side-bar").removeClass("hidden1");
		//var $container = $(".slide-container-without-margin");
		//$container.removeClass("slide-container-without-margin").addClass("slide-container");
	} else {
		$(".side-bar").addClass("hidden1");
		//var $container2 = $(".slide-container");
		//$container2.removeClass("slide-container").addClass("slide-container-without-margin");
	}
}