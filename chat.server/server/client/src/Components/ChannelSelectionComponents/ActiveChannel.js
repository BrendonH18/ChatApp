import { useParams } from "react-router-dom";
import { Button } from 'react-bootstrap'
import UserContainer from "./UserContainer";
import MessageContainer from "./MessageContainer";
import SendMessageForm from "./SendMessageForm";
import { useEffect, useState } from "react";
import { ConsoleLogger } from "@microsoft/signalr/dist/esm/Utils";

const ActiveChannel = ({ connection, messages, connectedUsers, userConnection, availableChannels }) => {
  
    const [isLoading, setIsLoading] = useState(true)
    let { ActiveChannelID } = useParams();
    // useEffect(() => {
    //   if(connection) {
    //     debugger
    //     // console.log(ActiveChannel)
    //     connection.send("JoinChannel", ActiveChannel)
    //   }
    // },[connection, ActiveChannel])

    useEffect(() => {
      let activeChannel = availableChannels[ActiveChannelID - 1 ]
      if(connection ){
        connection.send("JoinChannel", activeChannel)
        console.log(connection)
        console.log("WOOP!")
      }
      console.log(availableChannels[ActiveChannelID - 1])
      console.log(connection)
    }, [connection, ActiveChannelID])


    return(
      <>
      <h2>{ActiveChannelID}</h2>
      
      
      <div className='chat'>
      <h2>Chat for "{}"</h2>
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