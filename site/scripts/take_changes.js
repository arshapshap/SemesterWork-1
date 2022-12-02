function take_changes(source_id, receiver_id) {
    var source = document.getElementById(source_id);
    var receiver = document.getElementById(receiver_id);

    if (source != null && source.value != "" && receiver != null) {
        receiver.value = source.value;
        return true;
    }

    return false;
}