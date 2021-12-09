import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { useState } from 'react';
import Lobby from './Components/Lobby';
import Chat from './Components/Chat';

function App() {
  const [connection, setConnection] = useState();
  const [messages, setMessages] = useState([]);
  const [users, setUsers] = useState([]);
  const [roomName, setRoomName] = useState()
  const [activeRooms, setActiveRooms] = useState()
  
  const joinRoom = async (user, room) => {
    try {
      const localConnection = new HubConnectionBuilder()
        .withUrl('https://localhost:44314/chat') //add url
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build()

      localConnection.on("ReceiveActiveRooms", (rooms) => {
        setActiveRooms( rooms ) // pending
      })

      localConnection.on("ReceiveMessage", (user, text) => {
        setMessages(messages => [...messages, {user, text}]);
      })

      localConnection.on("ReceiveUsers", (users) => {
        setUsers( users );
      })

      localConnection.onclose(e =>{
        setConnection('');
        setMessages('');
        setUsers('');
        setRoomName('');
      })

      await localConnection.start();

      await localConnection.invoke("JoinRoom", {user, room})
      setRoomName(room);
      setConnection(localConnection);
    } catch (error) {
      console.log(error);
    }
  }

  const sendText = async (text) =>{
    try {
      await connection.invoke('SendMessage', text);
    } catch (error) {
      console.log(error);
    }
  }

  const leaveRoom = async () => {
    try {
      await connection.stop(); 
    } catch (error) {
      console.log(error);
    }
  }

  return (
    <div className='app container-fluid vh-100 d-flex flex-column '>
      {/* container-fluid h-100 */}
      <h2 className="d-flex justify-content-center">My Chat {
        roomName
          ? ` - ${roomName}`
          : ""}</h2>
      
      <hr className='line'/>

      {!connection 
        ?<Lobby 
          joinRoom={joinRoom}
          activeRooms={activeRooms}/>
        :<Chat
          sendText={sendText}
          messages={messages}
          leaveRoom={leaveRoom}
          users={users}/>
      }

    </div>
    
  );
}

export default App;
