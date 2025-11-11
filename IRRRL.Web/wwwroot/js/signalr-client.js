// SignalR client for real-time updates

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/application")
    .withAutomaticReconnect()
    .build();

connection.on("StatusChanged", function (data) {
    console.log("Application status changed:", data);
    // Trigger Blazor component update
    if (window.DotNet) {
        DotNet.invokeMethodAsync('IRRRL.Web', 'OnStatusChanged', data);
    }
});

connection.on("NewActionItem", function (data) {
    console.log("New action item:", data);
    if (window.DotNet) {
        DotNet.invokeMethodAsync('IRRRL.Web', 'OnNewActionItem', data);
    }
});

connection.on("DocumentUploaded", function (data) {
    console.log("Document uploaded:", data);
    if (window.DotNet) {
        DotNet.invokeMethodAsync('IRRRL.Web', 'OnDocumentUploaded', data);
    }
});

connection.on("DocumentValidated", function (data) {
    console.log("Document validated:", data);
    if (window.DotNet) {
        DotNet.invokeMethodAsync('IRRRL.Web', 'OnDocumentValidated', data);
    }
});

connection.on("EligibilityVerified", function (data) {
    console.log("Eligibility verified:", data);
    if (window.DotNet) {
        DotNet.invokeMethodAsync('IRRRL.Web', 'OnEligibilityVerified', data);
    }
});

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
}

connection.onclose(async () => {
    await start();
});

// Start the connection
start();

// Expose functions to Blazor
window.signalR = {
    subscribeToApplication: function (applicationId) {
        return connection.invoke("SubscribeToApplication", applicationId);
    },
    unsubscribeFromApplication: function (applicationId) {
        return connection.invoke("UnsubscribeFromApplication", applicationId);
    },
    subscribeToAllApplications: function () {
        return connection.invoke("SubscribeToAllApplications");
    }
};

