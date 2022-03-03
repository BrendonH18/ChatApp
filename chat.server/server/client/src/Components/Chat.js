import MessageContainer from "./MessageContainer"
import SendMessageForm from "./SendMessageForm"
import UserContainer from "./UserContainer"
import { Button } from 'react-bootstrap'

const Chat = ({ connection, setActiveRoom, setMessages, setUsers, messages, users }) => {
  
  const leaveRoom = async () => {
    try {
      await connection.invoke('LeaveRoom')
      setActiveRoom(null)
      setMessages([])
      setUsers([]) 
    } catch (error) {
      console.log(error);
    }
  }

  return(
    <div className='chat'>
      <div className="d-grid">
      <Button
      className="leave-room"
      variant="danger"
      onClick={ e => {
        e.preventDefault();
        leaveRoom();
      }}>Leave Chat</Button>
     </div>
     <div className='row'>
       <div className="col-3 d-grid">
      <UserContainer
      users={users}/>
      </div>

      <div className="col-9 d-grid">
      <MessageContainer 
      messages={messages}/>

      <SendMessageForm
      connection={connection} 
      />
      </div>
      </div>
    </div>
  )
}

export default Chat