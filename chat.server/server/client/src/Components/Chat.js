import MessageContainer from "./MessageContainer"
import SendMessageForm from "./SendMessageForm"
import UserContainer from "./UserContainer"
import { Button } from 'react-bootstrap'

const Chat = ({ connection, messages, connectedUsers }) => {
  
  const handleLeaveRoom = () => {
      connection.send('LeaveChannel')
  }

  return(
    <div className='chat'>
      <div className="d-grid">
      <Button
      className="leave-room"
      variant="danger"
      onClick={ e => {
        e.preventDefault();
        handleLeaveRoom();
      }}>Leave Chat</Button>
     </div>
     <div className='row'>
       <div className="col-3 d-grid">
      <UserContainer
      connectedUsers={connectedUsers}/>
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