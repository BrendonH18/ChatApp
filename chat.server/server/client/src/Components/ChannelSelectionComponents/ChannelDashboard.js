import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState, useRef } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import { faPaperPlane, faSearch } from "@fortawesome/free-solid-svg-icons"


const ChannelDashboard = ({ user, channel, availableChannels, messages, connectedUsers, connection, isConnectionLoading}) => {

const [connectedUsersByChannelAndStatus, setConnectedUsersByChannelAndStatus] = useState(null)
const [messageText, setMessageText] = useState('')
let navigate = useNavigate()
let { ActiveChannelID } = useParams();
const messagesEndRef = useRef(null)

const formatMessage = (message) => {
	if (parseInt(message.user.id) === parseInt(user.id)) return formatMessageSend(message)
	return formatMessageReceive(message)
}

const formatMessageSend = (message) => { return <>
	<div class="d-flex justify-content-start mb-4">
		<div class="img_cont_msg">
			<img src="https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg" class="rounded-circle user_img_msg"/>
		</div>
		<div class="msg_container">
			{message.text}
			<span class="msg_time">{message.created_on}</span>
		</div>
	</div>
	</>
}

const formatMessageReceive = (message) => { return <>
	<div class="d-flex justify-content-end mb-4">
		<div class="msg_container_send">
			{message.text}
			<span class="msg_time_send">{message.created_on}</span>
		</div>
		<div class="img_cont_msg">
			<img src="" class="rounded-circle user_img_msg"/>
		</div>
	</div>
	</>
}

const groupByChannelId = (objArray) => {
	if(!objArray) return
	let newArray = {}
	objArray.forEach(obj => {
		let key = obj["channel"]["id"]
		if(!newArray[key]){
			newArray[key] = {}
			newArray[key].Active = []
			newArray[key].Observe = []
		}
		if (obj.user.isPasswordValid) newArray[key].Active.push(obj)
		newArray[key].Observe.push(obj)
	});
	for (let i = 0; i < availableChannels.length; i++) {
		const channel = availableChannels[i];
		console.log("Channel: ", channel)
		if(!newArray[channel.id]){
			newArray[channel.id] = {}
			newArray[channel.id].Active = []
			newArray[channel.id].Observe = []
		}
	}
	return newArray
}

const handleChannelSelect = (channel) => {
	navigate(`/Channel/${channel.id}`)
}
const handleMessageInput = (e) => {
	setMessageText(e.target.value)
}
const handleSendMessage = (e) =>{
	e.preventDefault()
	if (!user.isPasswordValid) return setMessageText("Message blocked - Please login to send messages")
    const message = {
      text: messageText,
      isBot: false
    }
	setMessageText('')
	console.log("Message: ", message)
    connection.send("SendMessageToChannel", message);
  }

  const scrollToBottom = () => messagesEndRef.current?.scrollIntoView({behavior: "smooth"})
  
  useEffect(scrollToBottom, [messages])

  useEffect(() => {
	  const formattedUsers = groupByChannelId(connectedUsers)
	  setConnectedUsersByChannelAndStatus(formattedUsers)
  }, [connectedUsers])
	
  useEffect(() => {
	if(isConnectionLoading) return
	if(typeof ActiveChannelID === "undefined") return
	let id = parseInt(ActiveChannelID)
	if(id === 0) id = 1
	connection.send("JoinChannel", availableChannels[ id - 1 ])
  }, [isConnectionLoading, ActiveChannelID])



    return(
      <>
      <div class="container-fluid h-100">
			<div class="row justify-content-center h-100">
				<div class="col-md-3 col-xl-3 chat">
					<div class="card mb-sm-3 mb-md-0 contacts_card">
						<div class="card-header">
							<div class="input-group">
								<input type="text" placeholder="Search..." name="" class="form-control search"/>
								<div class="input-group-prepend">
									<span class="input-group-text search_btn h-100">
										<FontAwesomeIcon icon={faSearch}/>
									</span>
								</div>
							</div>
						</div>
						<div class="card-body contacts_body">
							<ui class="contacts">
								{availableChannels.map(channel =>
								<li class={parseInt(ActiveChannelID)===parseInt(channel.id) ? "active" : ""} onClick={e => handleChannelSelect(channel)}>
									<div class="d-flex bd-highlight" >
										<div class="img_cont" >
											<img src="https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg" class="rounded-circle user_img" />
											{connectedUsersByChannelAndStatus
												? <span class={connectedUsersByChannelAndStatus[channel.id]["Active"].length>0 ? "online_icon" : "online_icon offline"} ></span>
												: <></>}
										</div>
										<div class="user_info" >
											<span >{channel.name}</span>
											{connectedUsersByChannelAndStatus
												? <p>{`${connectedUsersByChannelAndStatus[channel.id]["Active"].length} Connected`}</p>
												: <></>}
										</div>
									</div>
								</li>)}
							</ui>
						</div>
						<div class="card-footer"></div>
					</div>
				</div>
				<div class="col-md-2 col-xl-2 chat">
					<div class="card">
						<div class="card-header msg_head">
							<div class="d-flex bd-highlight">
								<div class="img_cont">
									<img src="https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg" class="rounded-circle user_img"/>
									{connectedUsersByChannelAndStatus
										? <span class={connectedUsersByChannelAndStatus[channel.id]["Active"].length>0 ? "online_icon" : "online_icon offline"}></span>
										: <></>}
								</div>
								<div class="user_info">
									<span>Users</span>
									{connectedUsersByChannelAndStatus
										? <p>{`${connectedUsersByChannelAndStatus[ActiveChannelID]["Active"].length} Connected`}</p>
										: <></>}
								</div>
							</div>
						</div>
						<div class="card-body contacts_body">
							<ui class="contacts">
								{connectedUsersByChannelAndStatus
									? connectedUsersByChannelAndStatus[ActiveChannelID]["Active"].map(userConnection =>
								<li>
									<div class="d-flex bd-highlight" >
										<div class="user_info" >
											<span >{userConnection.user.username}</span>
										</div>
									</div>
								</li>
								)
									: <></>}
							</ui>
						</div>
					</div>

				</div>
				<div class="col-md-6 col-xl-6 chat">
					<div class="card">
						<div class="card-header msg_head">
							<div class="d-flex bd-highlight">
								<div class="img_cont">
									<img src="https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg" class="rounded-circle user_img"/>
									<span class="online_icon"></span>
								</div>
								<div class="user_info">
									<span>{`Let's Chat: ${channel.name}`}</span>
									<p>{`${messages.length} Message(s)`}</p>
								</div>
							</div>
						</div>
						<div class="card-body msg_card_body">
							{messages.map(message => { return formatMessage(message)})}
							<div ref={messagesEndRef}/>
						</div >
						<div class="card-footer">
							<div class="input-group">
								<div class="input-group-append">
									<span class="input-group-text attach_section h-100">
									</span>
								</div>
								<textarea name="" class="form-control type_msg" placeholder="Type your message..." onChange={e => handleMessageInput(e)} value={messageText}></textarea>
								<div class="input-group-append">
									<span class="input-group-text send_btn h-100" onClick={e => handleSendMessage(e)}>
										<FontAwesomeIcon icon={faPaperPlane} 
										color={user.isPasswordValid
											? "#90EE90"
											: "#ff4040"}
										/>
									</span>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
      </>
    )
  }

  export default ChannelDashboard;