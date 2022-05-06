import { useParams } from "react-router-dom";
import { Button } from 'react-bootstrap'
import UserContainer from "./UserContainer";
import MessageContainer from "./MessageContainer";
import SendMessageForm from "./SendMessageForm";
import { useEffect, useState } from "react";
import { ConsoleLogger } from "@microsoft/signalr/dist/esm/Utils";

const ActiveChannel = ({ connection, messages, isConnectionLoading, connectedUsers, userConnection, availableChannels }) => {

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
      if(connection){
        console.log(activeChannel)
        connection.send("JoinChannel", activeChannel)
      }
    }, [isConnectionLoading, ActiveChannelID])


    return(
      <>
      <h2>{ActiveChannelID}</h2>


      <div className='chat'>
      <h2>Chat for "{}"</h2>
      <div className="d-grid">
      {/* <Button
      className="leave-room"
      variant="danger"
      onClick={ e => {
        e.preventDefault();
        console.log("Leave Room Pressed");
      }}>Leave Chat</Button> */}
     </div>
     <div className='row'>
       <div className="col-3 d-grid">
      <UserContainer
      connectedUsers={connectedUsers}/>
      </div>

      <div className="col-9 d-grid">
      <MessageContainer
      messages={messages}/>

      <SendMessageForm connection={connection}
      userConnection={userConnection}
      />
      </div>
      </div>
    </div>

      </>
    )
  }

  export default ActiveChannel;