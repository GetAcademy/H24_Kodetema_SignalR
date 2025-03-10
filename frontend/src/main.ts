import './style.css';
import * as signalR from '@microsoft/signalr';

let connectedUsers: string[] = [];
let documents: Document[] = [];
let amountOfRefreshes: number = 0;
const apiUrl = "https://localhost:7250";
const myId: string = crypto.randomUUID().toString();
const connection = new signalR.HubConnectionBuilder().withUrl(`${apiUrl}/documentHub`).build();

connection.on("OpenDocumentsChanged", async (openDocuments: Document[])=> {
   documents = openDocuments;
   amountOfRefreshes++;
   await renderDocuments();
});

connection.on("ConnectedUsersChanged", (users: string[]) => {
    connectedUsers = users;
    renderConnectedUsers();
});

connection.on("ShowNotification", (message: string) => {
   showToast(message); 
});

(async () => {
    await connection.start().catch((err) => console.log(err));
    await connection.send("RegisterId", myId);
})();

interface Document {
    id: number;
    completed: boolean;
    createdAt: string;
    owner: string | null;
    content: string;
    fileName: string;
}

async function updateOwner(id: number, ownerName: string) {
    try {        
        await connection.send('UpdateOwner', id, ownerName);
        await renderDocuments();
    } catch (error) {
        console.error('Error updating owner:', error);
    }
}

async function releaseOwner(id: number) {
    try {        
        await connection.send('ReleaseOwner', id);
        await renderDocuments();
    } catch (error) {
        console.error('Error releasing owner:', error);
    }
}

async function markAsCompleted(id: number) {
    try {        
        await connection.send('MarkAsCompleted', id);
        await renderDocuments();
    } catch (error) {
        console.error('Error marking document as completed:', error);
    }
}

async function nudgeUser(userId: string) {
    try {
        await connection.send('NudgeUser', userId);        
    } catch (error) {
        console.error('Error nudging user:', error);
    }
}

function showToast(message: string) {
    const toast = document.createElement("div");
    toast.className = "toast";
    toast.innerText = message;
    document.body.appendChild(toast);
    
    setTimeout(() => {
        toast.remove();
    }, 3000);
}

function renderConnectedUsers() {
    const usersDiv = document.querySelector<HTMLDivElement>('#connected-users')!;
    usersDiv.innerHTML = `
        <h2>Connected users</h2>
        <ul>
            ${connectedUsers.map(user => `<li>${user}<button onclick="nudgeUser('${user}')" ${myId === user ? 'disabled' : ''}>Nudge</button></li>`).join('')}
        </ul>
    `;
}

async function renderDocuments() {    
    const appDiv = document.querySelector<HTMLDivElement>('#app')!;

    appDiv.innerHTML = `
    <h1>Document Manager</h1>
    ${myId}
    <div>Refresh count: ${amountOfRefreshes}</div>
    <div id="connected-users"></div>
    <ul>
      ${documents.map(doc => `
        <li>
          <div class="document-info">
            <i class="fas fa-file-alt"></i>
            <div>
              <span class="file-name">${doc.fileName}</span>
              <span class="owner"><strong>Owner:</strong> ${doc.owner ?? 'None'}</span>
            </div>
          </div>
          <div class="button-group">
            <button ${doc.owner ? 'disabled' : ''} onclick="updateOwner(${doc.id}, myId)"><i class="fas fa-user-edit"></i> Update Owner</button>
            <button onclick="releaseOwner(${doc.id})"><i class="fas fa-user-slash"></i> Release Owner</button>
            <button class="button-complete" onclick="markAsCompleted(${doc.id})"><i class="fas fa-check"></i> Complete</button>
          </div>
        </li>
      `).join('')}
    </ul>
  `;
    renderConnectedUsers();
}

renderDocuments();

(window as any).updateOwner = updateOwner;
(window as any).releaseOwner = releaseOwner;
(window as any).markAsCompleted = markAsCompleted;
(window as any).myId = myId;
(window as any).renderDocuments = renderDocuments;
(window as any).showToast = showToast;
(window as any).renderConnectedUsers = renderConnectedUsers;
(window as any).nudgeUser = nudgeUser;