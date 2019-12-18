/**$(window).load(function () {
    $("#loadContainer > img").fadeOut(500);
    //document.querySelector("#loadingSpinner").style.display = "none"; //makes page more lightweight     
});
**/

$(function() {
	/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//                     DISABLE SUBMIT INPUT TO DELETE WHEN NO CLAIMS
	/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	$(document).ready(function() {
		document.getElementById("loadingGears").style.display = "none";

		var listCount = $("#myTable tbody tr").length;

		if (listCount < 1) {
			$("input[name='removeSelected']").attr("disabled", true);
			$("#select-all").attr("disabled", true);
		}
		else {
			$("input[name='removeSelected']").attr("disabled", false);
			$("#select-all").attr("disabled", false);
		}
	});

	/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	///                                    BUTTON
	/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	$("#select-all").click(function() {
		var all = $("#select-all")[0];
		all.checked = !all.checked; // toggle
		var checked = all.checked;
		$("input.select-item").each(function(index, item) {
			item.checked = checked;
		});
	});

	/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	///                                      INPUT
	/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	$("input.select-item").click(function() {
		var checked = this.checked;
		console.log(checked);

		var all = $("input.select-all")[0]; // checkSelected();
		var total = $("input.select-item").length;
		var len = $("input.select-item:checked:checked").length;
		all.checked = len === total;
	});
});
