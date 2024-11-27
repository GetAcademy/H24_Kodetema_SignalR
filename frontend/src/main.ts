import './style.css';
import axios from 'axios';
import * as signalR from '@microsoft/signalr';



let documents: Document[] = [];
const apiUrl = "https://localhost:7250";
const myId: string = crypto.randomUUID().toString();
const connection = new signalR.HubConnectionBuilder().withUrl(`${apiUrl}/documentHub`).build();

connection.on("OpenDocumentsChanged", async (openDocuments: Document[])=> {
   documents = openDocuments;
   await renderDocuments();
});

await connection.start().catch(err => console.log(err));

await connection.send('RegisterId', myId);

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
        // await axios.put(`${apiUrl}/document/updateOwner/${id}/${ownerName}`);
        await connection.send('UpdateOwner', id, ownerName);
        await renderDocuments();
    } catch (error) {
        console.error('Error updating owner:', error);
    }
}

async function releaseOwner(id: number) {
    try {
        // await axios.put(`${apiUrl}/document/updateOwner/${id}/release`);
        await connection.send('ReleaseOwner', id);
        await renderDocuments();
    } catch (error) {
        console.error('Error releasing owner:', error);
    }
}

async function markAsCompleted(id: number) {
    try {
        // await axios.put(`${apiUrl}/document/complete/${id}`);
        await connection.send('MarkAsCompleted', id);
        await renderDocuments();
    } catch (error) {
        console.error('Error marking document as completed:', error);
    }
}

async function renderDocuments() {
    // const documents = await fetchOpenDocuments();
    const appDiv = document.querySelector<HTMLDivElement>('#app')!;

    appDiv.innerHTML = `
    <h1>Document Manager</h1>
    ${myId}
    <button onclick="renderDocuments()"><i class="fas fa-refresh"></i>Refresh</button>    
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
}

renderDocuments();

(window as any).updateOwner = updateOwner;
(window as any).releaseOwner = releaseOwner;
(window as any).markAsCompleted = markAsCompleted;
(window as any).myId = myId;
(window as any).renderDocuments = renderDocuments;