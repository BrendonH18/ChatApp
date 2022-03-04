import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { useEffect, useState } from 'react';
import Lobby from './Components/Lobby';
import Chat from './Components/Chat';
import Login from './Components/Login';
// import bcrypt from 'bcrypt';

function App() {
  const [messages, setMessages] = useState([]);
  const [users, setUsers] = useState([]);
  const [activeRoom, setActiveRoom] = useState(null)
  const [availableRooms, setAvailableRooms] = useState(null);
  const [isValid, setIsValid] = useState(false)
  const [userName, setUserName] = useState('')
  const [connection, setConnection] = useState(null)
  const [loginMessage, setLoginMessage] = useState('')

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
    .withUrl('https://localhost:44314/chat')
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();
    setConnection(newConnection)
  },[])

  useEffect(() => {
    if(connection){
      connection.start()
      .then(result => {
        connection.on("ReturnedIsValid", (param) => handleReturnedIsValid(param))
        connection.on("ReturnedMessage", (param) => setMessages(messages => [...messages, {param}])) 
        connection.on("ReturnedUsers", (param) => setUsers( param ))
        connection.on("ReturnedAvailableRooms", (param) => setAvailableRooms( param ))

        connection.send("ReturnAvailableRooms")

        connection.onclose(e =>{
          setMessages('');
          setUsers('');
          setUserName('');
        })
      })
      .catch(e => console.log(e))
    }
  },[connection])

  const handleReturnedIsValid = (param) => {
    setIsValid(param.isValid);
    if (param.IsValid === false) return;
    setUserName(param.username);
    setLoginMessage(param.loginmessage)
  }

  useEffect(() => {
    chatBody()
  },[isValid])

  const chatBody = () => {
    if (!isValid) return <Login connection={connection}/>
    return !activeRoom 
      ?<Lobby
        connection={connection}
        setIsValid={setIsValid}
        setUserName={setUserName}
        setLoginMessage={setLoginMessage}
        setActiveRoom={setActiveRoom}
        availableRooms={availableRooms}
        userName={userName}/>
      :<Chat
        connection={connection}
        setActiveRoom={setActiveRoom}
        setMessages={setMessages}
        setUsers={setUsers}
        messages={messages}
        users={users}/>
  }

  return (
    <div className='app container-fluid vh-100 d-flex flex-column '>
      <h2 className="d-flex justify-content-center">My Chat {
        activeRoom
          ? ` - ${activeRoom}`
          : ""}</h2>
      <hr className='line'/>
      {chatBody()}
    </div>
  );
}

export default App;
