
// function that gathers IDs of checked nodes
function checkedNodesIds(nodes, checkedNodes) {
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].checked) {
            checkedNodes.push(nodes[i].id);
        }

        if (nodes[i].hasChildren) {
            checkedNodesIds(nodes[i].children.view(), checkedNodes);
        }
    }
}

(function () {

    $(function () {

        //animate numbers
        $('[data-animate-number]').each(function () {

            var $this = $(this);
            jQuery({ Counter: 0 }).animate({ Counter: $this.text() }, {
                duration: 2900,
                easing: 'swing',
                step: function () {
                    $this.text(Math.ceil(this.Counter));
                }
            });

        });

    });

})();