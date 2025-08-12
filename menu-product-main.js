function imageOnChanged(executionContext) {
    debugger;
    const formContext = executionContext.getFormContext();
    const image = formContext.getAttribute("uls_productimage").getValue();

    if (image) {

        formContext.getAttribute("uls_imagesyncdate").setValue(null);
        formContext.data.save(1).then(
            function () {
                // Optionally, you can add code here to handle success
            },
            function (error) {
                console.error("Error saving the form: ", error);
            }
        );
    }
}