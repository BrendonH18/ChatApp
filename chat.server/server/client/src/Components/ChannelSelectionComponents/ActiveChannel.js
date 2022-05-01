import { useParams } from "react-router-dom";
import { Button } from 'react-bootstrap'
import UserContainer from "./UserContainer";
import MessageContainer from "./MessageContainer";
import SendMessageForm from "./SendMessageForm";

const ActiveChannel = ({ messages, connectedUsers, userConnection }) => {
  
    let { ActiveChannel } = useParams();

    return(
      <>
      
      
      <div className='chat'>
      <h2>Chat for "{ActiveChannel}"</h2>
      <div className="d-grid">
      <Button
      className="leave-room"
      variant="danger"
      onClick={ e => {
        e.preventDefault();
        console.log("Leave Room Pressed");
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
      userConnection={userConnection} 
      />
      </div>
      </div>
    </div>
      
      </>
    )
  }
  
  export default ActiveChannel;