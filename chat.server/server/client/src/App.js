import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { useEffect, useState } from 'react';
import Lobby from './Components/Lobby';
import Chat from './Components/Chat';
import Login from './Components/Login';
// import bcrypt from 'bcrypt';

function App() {
  // const [connection, setConnection] = useState();
  const [messages, setMessages] = useState([]);
  const [users, setUsers] = useState([]);
  const [activeRoom, setActiveRoom] = useState()
  const [isvalid, setIsvalid] = useState(null)
  const [userName, setUserName] = useState()
  const [loConnection, setLoConnection] = useState(null)

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
    .withUrl('https://localhost:44314/chat')
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();
    setLoConnection(newConnection)
  },[])

  useEffect(() => {
    if(loConnection){
      loConnection.start()
      .then(result => {
        loConnection.on("ReturnedIsValid", (param) => {
          console.log(param)  
          setIsvalid(param.isValid);
            if (param.IsValid === false) return;
            setUserName(param.username);
        })

        // loConnection.on("ReceiveActiveRooms", (rooms) => {
        //   setActiveRooms( rooms ) // pending
        // })
  
        loConnection.on("ReturnedMessage", (param) => setMessages(messages => [...messages, {param}])) 
        loConnection.on("ReturnedUsers", (param) => setUsers( param ));
  
        loConnection.onclose(e =>{
          // setConnection('');
          setMessages('');
          setUsers('');
          // setRoomName('');
          setUserName('');
        })


      })
      .catch(e => console.log(e))
    }
  },[loConnection])

  const userValidation = (param) => {
    try {
      loConnection.send("ReturnIsValid", param)
    } catch (error) {
      console.log(error)
    }    
  }

  const activateChat = async (userName, activeRoom) => {
    try {
      await loConnection.invoke("JoinRoom", {userName, activeRoom} )
      setActiveRoom(activeRoom);
    } catch (error) {
      console.log(error);
    }
  }

  const sendText = async (text) =>{
    try {
      await loConnection.send('SendMessage', text);
    } catch (error) {
      console.log(error);
    }
  }

  const leaveRoom = () => {
    try {
      sendText(`${userName} has left the room`)
      setActiveRoom()
      setMessages([])
      setUsers([]) 
    } catch (error) {
      console.log(error);
    }
  }

  const Validation = () => {
    if(isvalid === null) return <div><h2>Please Login</h2><Login
    userValidation={userValidation} /></div>
    if(isvalid === false) return <div><h2>Credentials Don't Match</h2><Login userValidation={userValidation} /></div>
    return <h2>Welcome {userName}</h2>
  }

  return (
    <div className='app container-fluid vh-100 d-flex flex-column '>
      {/* container-fluid h-100 */}
      <h2 className="d-flex justify-content-center">My Chat {
        activeRoom
          ? ` - ${activeRoom}`
          : ""}</h2>
      
      <hr className='line'/>

      <Validation/>

      {!activeRoom 
        ?<Lobby 
          joinRoom={activateChat}
          // activeRooms={activeRooms}
          userName={userName}/>
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
