function removeNestedForm(element, container, deleteElement) {
    $container = $(element).parent();
    $container.find(deleteElement).val('True');
    $container.hide();
}

function addNestedForm(element, counter, fakeindex, content) {
    var nextIndex = $(counter).length;
    var pattern = new RegExp(fakeindex, "gi");
    content = content.replace(pattern, nextIndex);
    var container = $(element).parent().prev();
    $(container).append(content);
    $(container).children(counter).last().find("input.mark-for-insert").val('True');
}