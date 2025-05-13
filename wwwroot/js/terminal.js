function htmlEscape(text) {
    return text.replace(/[&<>"']/g, function (match) {
        return {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        }[match];
    });
}

function loadServers() {
    $.getJSON("/connections", function (data) {
        const select = $('#serverSelect');
        select.empty();
        select.append('<option value="">- Server auswählen -</option>');
        data.forEach(server => {
            const option = $('<option>').val(server.value).text(server.name);
            select.append(option);
        });
    }).fail(function () {
        alert("Fehler beim Laden der Serverliste.");
    });
}

function loadPreferences() {
    return $.getJSON("/api/UserPreferences")
        .done(prefs => {
            $('#colorPicker').val(prefs.color);
            $('#terminal').css('color', prefs.color);
            $('.cursor').css('background-color', prefs.color);

            $('#fontSizeRange').val(prefs.fontSize);
            $('#fontSizeValue').text(prefs.fontSize);
            $('#terminal').css('font-size', prefs.fontSize + 'px');
        })
        .fail(() => {
            console.warn("Konnte Benutzerpräferenzen nicht laden.");
        });
}

function savePreferences() {
    const prefs = {
        color: $('#colorPicker').val(),
        fontSize: parseInt($('#fontSizeRange').val(), 10)
    };

    return $.ajax({
        url: '/api/UserPreferences',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(prefs)
    }).fail(() => {
        console.warn("Fehler beim Speichern der Benutzerpräferenzen.");
    });
}

let connection;
let commandBuffer = '';

function initializeSignalR() {
    connection = new signalR.HubConnectionBuilder().withUrl('/ssh').build();

    connection.start()
        .then(() => {
            console.log("SignalR-Verbindung hergestellt");
            initializeTerminal();
        })
        .catch(err => console.error(err.toString()));

    connection.on('ReceiveOutput', function (message) {
        appendToTerminal(message);
    });
}

function appendToTerminal(text) {
    $('#input-line').remove();
    $('.cursor').remove();
    $('#terminal').append(htmlEscape(text));
    appendPrompt();
    scrollToBottom();
}

function appendPrompt() {
    const inputLine = $('<span id="input-line" class="input-line"></span>');
    const cursor = $('<span class="cursor">&nbsp;</span>').css({
        'display': 'inline-block',
        'width': '10px',
        'background-color': $('#colorPicker').val(),
        'animation': 'blink 1s step-end infinite'
    });
    $('#terminal').append(inputLine).append(cursor);
    scrollToBottom();
}

function initializeTerminal() {
    $('#terminal').empty();
    appendPrompt();
}

function scrollToBottom() {
    const terminal = $('#terminal');
    terminal.scrollTop(terminal[0].scrollHeight);
}

function initializeEventListeners() {
    const connectButton = $('#connectButton');
    const serverSelect = $('#serverSelect');
    const connectionAddButton = $('#connectionAddButton');
    const colorPicker = $('#colorPicker');
    const fontSizeRange = $('#fontSizeRange');
    const fontSizeValue = $('#fontSizeValue');

    connectButton.click(function () {
        const selectedServer = serverSelect.val();
        if (selectedServer.trim()) {
            connection.invoke('Connect', selectedServer).catch(err => console.error(err.toString()));
        } else {
            alert("Bitte wähle einen Server aus.");
        }
    });

    connectionAddButton.click(function () {
        const serverHost = prompt("Host-Adresse:");
        const serverPort = parseInt(prompt("Port (Standard: 22):") || "22", 10);
        const serverUsername = prompt("Benutzername:");
        const serverPassword = prompt("Passwort:");
        const serverSudoPassword = prompt("Sudo-Passwort (optional):");

        if (serverHost && serverPort && serverUsername && serverPassword) {
            const newServer = {
                Host: serverHost,
                Port: serverPort,
                Username: serverUsername,
                Password: serverPassword,
                SudoPassword: serverSudoPassword
            };

            $.ajax({
                url: "addServer",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(newServer),
                success: function () {
                    alert(`Server hinzugefügt:\nHost: ${newServer.Host}\nPort: ${newServer.Port}`);
                    loadServers();
                },
                error: function (xhr) {
                    alert("Fehler beim Hinzufügen: " + xhr.responseText);
                }
            });
        } else {
            alert("Bitte alle Pflichtfelder ausfüllen.");
        }
    });

    $(document).on('keydown', function (e) {
        const inputLine = $('#input-line');
        if (e.key === 'Backspace') {
            e.preventDefault();
            commandBuffer = commandBuffer.slice(0, -1);
            inputLine.text(commandBuffer);
        } else if (e.key === 'Enter' && commandBuffer.length > 0) {
            e.preventDefault();
            connection.invoke('SendCommand', commandBuffer).catch(err => console.error(err.toString()));
            commandBuffer = '';
            $('#input-line').remove();
            $('.cursor').remove();
            appendPrompt();
        } else if (e.key.length === 1 && !e.ctrlKey && !e.metaKey) {
            e.preventDefault();
            commandBuffer += e.key;
            inputLine.text(commandBuffer);
        }
        scrollToBottom();
    });

    colorPicker.on('input', function () {
        const color = $(this).val();
        $('#terminal').css('color', color);
        $('.cursor').css('background-color', color);
        savePreferences();
    });

    fontSizeRange.on('input', function () {
        const size = $(this).val();
        $('#terminal').css('font-size', size + 'px');
        fontSizeValue.text(size);
        savePreferences();
    });
}

$(function () {
    loadServers();
    loadPreferences().then(() => {
        initializeSignalR();
        initializeEventListeners();
    });
});
