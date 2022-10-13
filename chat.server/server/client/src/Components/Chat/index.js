import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState, useRef } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import { faPaperPlane, faSearch } from "@fortawesome/free-solid-svg-icons"
import axios from 'axios'
import * as methods from "./Methods"
import Channels from "./Components/Channels";
import Users from "./Components/Users";
import Messages from "./Components/Messages";


const Chat = ({ jwt, user, channel, availableChannels, messages, connectedUsers, connection, firstChannelId}) => {

const [connectedUsersByChannelAndStatus, setConnectedUsersByChannelAndStatus] = useState(null)
let { ActiveChannelID } = useParams();

const isConnectedUsersByChannelAndStatusTruthy = () =>{
	try {
		const value = connectedUsersByChannelAndStatus[channel.id]["Active"].length>0
	} catch (error) {
		return false
	}
	return true
	return !!connectedUsersByChannelAndStatus && connectedUsersByChannelAndStatus[channel.id] && connectedUsersByChannelAndStatus[channel.id]["Active"]
}

  useEffect(() => {
	  const formattedUsers = methods.groupUsers(connectedUsers,availableChannels)
	  setConnectedUsersByChannelAndStatus(formattedUsers)
  }, [connectedUsers])

    return(
      <>
      <div className="container-fluid mh-100">
			<div className="row justify-content-center mh-100">
				<div className="col-md-3 col-xl-3 chat">
					<Channels connection={connection} channel={channel} availableChannels={availableChannels} ActiveChannelID={ActiveChannelID} isConnectedUsersByChannelAndStatusTruthy={isConnectedUsersByChannelAndStatusTruthy} connectedUsersByChannelAndStatus={connectedUsersByChannelAndStatus}/>
				</div>
				<div className="col-md-2 col-xl-2 chat">
					<Users isConnectedUsersByChannelAndStatusTruthy={isConnectedUsersByChannelAndStatusTruthy} connectedUsersByChannelAndStatus={connectedUsersByChannelAndStatus}/>
				</div>
				<div className="col-md-6 col-xl-6 chat">
					<Messages jwt={jwt} channel={channel} user={user} messages={messages}/>
				</div>
			</div>
		</div>
      </>
    )
  }

  export default Chat;