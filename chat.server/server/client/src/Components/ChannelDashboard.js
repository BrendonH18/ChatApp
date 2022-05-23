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
	<div className="d-flex justify-content-start mb-4">
		<div className="img_cont_msg">
			<img prop="" src="https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg" className="rounded-circle user_img_msg"/>
		</div>
		<div className="msg_container">
			{message.text}
			<span className="msg_time">{message.created_on}</span>
		</div>
	</div>
	</>
}

const formatMessageReceive = (message) => { return <>
	<div className="d-flex justify-content-end mb-4">
		<div className="msg_container_send">
			{message.text}
			<span className="msg_time_send">{message.created_on}</span>
		</div>
		<div className="img_cont_msg">
			<img prop="" src="" className="rounded-circle user_img_msg"/>
		</div>
	</div>
	</>
}

const groupByChannelId = (objArray) => {
	if(!objArray) return
	if(!availableChannels) return
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
	if(!availableChannels) return
	if(typeof ActiveChannelID === "undefined") return
	let id = parseInt(ActiveChannelID)
	if(id === 0) id = 1
	if(id === parseInt(channel.id)) return
	connection.send("JoinChannel", availableChannels[ id - 1 ])
  }, [ActiveChannelID, availableChannels])

    return(
      <>
      <div className="container-fluid h-100">
			<div className="row justify-content-center h-100">
				<div className="col-md-3 col-xl-3 chat">
					<div className="card mb-sm-3 mb-md-0 contacts_card">
						<div className="card-header">
							<div className="input-group">
								<input type="text" placeholder="Search..." name="" className="form-control search"/>
								<div className="input-group-prepend">
									<span className="input-group-text search_btn h-100">
										<FontAwesomeIcon icon={faSearch}/>
									</span>
								</div>
							</div>
						</div>
						<div className="card-body contacts_body">
							<ul className="contacts">
								{availableChannels
									? availableChannels.map(channel =>
								<li key={`channel-${channel.id}`} className={parseInt(ActiveChannelID)===parseInt(channel.id) ? "active" : ""} onClick={e => handleChannelSelect(channel)}>
									<div className="d-flex bd-highlight" >
										<div className="img_cont" >
											<img src="https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg" className="rounded-circle user_img" />
											{connectedUsersByChannelAndStatus
												? <span className={connectedUsersByChannelAndStatus[channel.id]["Active"].length>0 ? "online_icon" : "online_icon offline"} ></span>
												: <></>}
										</div>
										<div className="user_info" >
											<span >{channel.name}</span>
											{connectedUsersByChannelAndStatus
												? <p>{`${connectedUsersByChannelAndStatus[channel.id]["Active"].length} Connected`}</p>
												: <></>}
										</div>
									</div>
								</li>)
									: <></>}
							</ul>
						</div>
						<div className="card-footer"></div>
					</div>
				</div>
				<div className="col-md-2 col-xl-2 chat">
					<div className="card">
						<div className="card-header msg_head">
							<div className="d-flex bd-highlight">
								<div className="img_cont">
									<img src="https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg" className="rounded-circle user_img"/>
									{connectedUsersByChannelAndStatus
										? <span className={connectedUsersByChannelAndStatus[channel.id]["Active"].length>0 ? "online_icon" : "online_icon offline"}></span>
										: <></>}
								</div>
								<div className="user_info">
									<span>Users</span>
									{connectedUsersByChannelAndStatus
										? <p>{`${connectedUsersByChannelAndStatus[ActiveChannelID]["Active"].length} Connected`}</p>
										: <></>}
								</div>
							</div>
						</div>
						<div className="card-body contacts_body">
							<ul className="contacts">
								{connectedUsersByChannelAndStatus
									? connectedUsersByChannelAndStatus[ActiveChannelID]["Active"].map(userConnection =>
								<li id={`contact-${userConnection.user.id}`}>
									<div className="d-flex bd-highlight" >
										<div className="user_info" >
											<span >{userConnection.user.username}</span>
										</div>
									</div>
								</li>
								)
									: <></>}
							</ul>
						</div>
					</div>

				</div>
				<div className="col-md-6 col-xl-6 chat">
					<div className="card">
						<div className="card-header msg_head">
							<div className="d-flex bd-highlight">
								<div className="img_cont">
									<img src="https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg" className="rounded-circle user_img"/>
									<span className="online_icon"></span>
								</div>
								<div className="user_info">
									<span>{`Let's Chat: ${channel.name}`}</span>
									{messages
										? <p>{`${messages.length} Message(s)`}</p>
										: <></>}
								</div>
							</div>
						</div>
						<div className="card-body msg_card_body">
							{messages
								? messages.map(message => { return formatMessage(message)})
								: <></>}
							<div ref={messagesEndRef}/>
						</div >
						<div className="card-footer">
							<div className="input-group">
								<div className="input-group-append">
									<span className="input-group-text attach_section h-100">
									</span>
								</div>
								<textarea name="" className="form-control type_msg" placeholder="Type your message..." onChange={e => handleMessageInput(e)} value={messageText}></textarea>
								<div className="input-group-append">
									<span className="input-group-text send_btn h-100" onClick={e => handleSendMessage(e)}>
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