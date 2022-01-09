import MessageContainer from "./MessageContainer"
import SendMessageForm from "./SendMessageForm"
import UserContainer from "./UserContainer"
import { Button } from 'react-bootstrap'

const Chat = ({ sendText, messages, leaveRoom, users }) => {

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
      sendText={sendText}/>
      </div>
      </div>
    </div>
  )
}

export default Chat