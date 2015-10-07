/*
	A script for checkboxes controlling whether a given element is visible.
	To use add "data-visibility-control-for = {selector}" to a checkbox to specify what element should be controlled.
*/

$(function()
{
	function getControlledElement(checkbox)
	{
		return $(checkbox).attr(attributeName);
	}

	var attributeName   = "data-visibility-control-for";
	var checkBoxes      = $("[" + attributeName + "]");
	var controlledItems = checkBoxes.map(function(i, element) { return getControlledElement(element); });

	controlledItems.each(function(i, element)
	{
		$(element).hide();
	});

	checkBoxes.on("change", function()
	{
		var $controlledElement = $(getControlledElement(this));
		if (this.checked) {
			$controlledElement.slideDown(300);
		} else {
			$controlledElement.slideUp(300);
		}
	});
})
