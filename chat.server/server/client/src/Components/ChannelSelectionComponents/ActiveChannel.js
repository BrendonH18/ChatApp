import { useParams } from "react-router-dom";
import { Button } from 'react-bootstrap'
import UserContainer from "./UserContainer";
import MessageContainer from "./MessageContainer";
import SendMessageForm from "./SendMessageForm";
import { useEffect, useState } from "react";
import { ConsoleLogger } from "@microsoft/signalr/dist/esm/Utils";

const ActiveChannel = ({ connection, messages, isConnectionLoading, connectedUsers, userConnection, availableChannels }) => {

    let { ActiveChannelID } = useParams();
    const [activeChannelName, setActiveChannelName] = useState("")


    useEffect(() => {
      let activeChannel = availableChannels[ActiveChannelID - 1 ]
      if(connection){
        connection.send("JoinChannel", activeChannel)
        setActiveChannelName(activeChannel.name)
      }
    }, [isConnectionLoading, ActiveChannelID])

    return(
      <>
      <h2>Chatroom: {activeChannelName}</h2>

      <div className='chat'>
      <div className="d-grid">
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