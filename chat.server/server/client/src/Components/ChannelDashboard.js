import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState, useRef } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import { faPaperPlane, faSearch } from "@fortawesome/free-solid-svg-icons"


const ChannelDashboard = ({ user, channel, availableChannels, messages, connectedUsers, connection, firstChannelId}) => {

const [connectedUsersByChannelAndStatus, setConnectedUsersByChannelAndStatus] = useState(null)
const [messageText, setMessageText] = useState('')
const [search, setSearch] = useState("")
const [trimChannels, setTrimChannels] = useState(availableChannels)

let navigate = useNavigate()
let { ActiveChannelID } = useParams();
const messagesEndRef = useRef(null)

const formatDateTime = (dt) => {
	const dateTime = new Date(dt)
	const timeDate_DifferenceInSeconds = Math.ceil((Date.now() - dateTime) / 1000)
	if (timeDate_DifferenceInSeconds <= 59) return `${timeDate_DifferenceInSeconds} second${timeDate_DifferenceInSeconds<=1?"":"s"} ago`
	if (timeDate_DifferenceInSeconds <= 360) return `${Math.floor(timeDate_DifferenceInSeconds / 60)} minutes ago`
	if (timeDate_DifferenceInSeconds <= 60*60*24) return `About ${Math.ceil(timeDate_DifferenceInSeconds / 60 / 60)} hour${Math.ceil(timeDate_DifferenceInSeconds / 60 / 60) <= 1?"":"s"} ago`
	return dateTime.toLocaleString()
}

const formatMessage = (message) => {
	if (parseInt(message.user.id) === parseInt(user.id)) return formatMessageSend(message)
	return formatMessageReceive(message)
}

const formatMessageSend = (message) => { return <>
	<div className="d-flex justify-content-start mb-4">
		<div className="img_cont_msg">
			{/* Image From: https://www.clipartmax.com/so/user-profile-icon/ */}
			<img prop="" src="https://www.clipartmax.com/png/small/293-2931307_account-avatar-male-man-person-profile-icon-profile-icons.png" className="rounded-circle user_img_msg"/>
		</div>
		<div className="msg_container">
			<span className="msg_user">{message.user.username}</span>
			{message.text}
			<span className="msg_time">{formatDateTime(message.created_on)}</span>
		</div>
	</div>
	</>
}

const formatMessageReceive = (message) => { return <>
	<div className="d-flex justify-content-end mb-4">
		<div className="msg_container_send">
			<span className="msg_user_send">{message.user.username}</span>
			{message.text}
			<span className="msg_time_send">{formatDateTime(message.created_on)}</span>
		</div>
		<div className="img_cont_msg">
			<img prop="" src="https://www.clipartmax.com/png/small/171-1717870_stockvader-predicted-cron-for-may-user-profile-icon-png.png" alt="Stockvader Predicted Cron For May - User Profile Icon Png @clipartmax.com" className="rounded-circle user_img_msg"/>
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

const isConnectedUsersByChannelAndStatusTruthy = () =>{
	return connectedUsersByChannelAndStatus && connectedUsersByChannelAndStatus[channel.id] && connectedUsersByChannelAndStatus[channel.id]["Active"]
}

const handleMessageInput = (e) => {
	setMessageText(e.target.value)
}

const handleChannelSelect = (channel) => {
	navigate(`/Channel/${channel.id}`)
}

const handleMessageSubmitWithEnterKey = (e) => {
	if (!user.isPasswordValid) return
	if (messageText === "") return
	if (e.code === "Enter") handleSendMessage(e)
}
const handleSendMessage = (e) =>{
	e.preventDefault()
	if (!user.isPasswordValid) return setMessageText("Message blocked - Please login to send messages")
    const message = {
      text: messageText,
      isBot: false
    }
	setMessageText('')
    connection.send("SendMessageToChannel", message);
  }

  const scrollToBottom = () => messagesEndRef.current?.scrollIntoView({behavior: "smooth"})

  const trimAvailableChannels = () => {
	  if (search === "") return setTrimChannels(availableChannels)
	  setTrimChannels( availableChannels.filter(ch => ch.name.toLowerCase().includes(search.toLowerCase())))
  }

  
  
  useEffect(scrollToBottom, [messages])

  useEffect(trimAvailableChannels,[search, availableChannels])

  useEffect(() => {
	  const formattedUsers = groupByChannelId(connectedUsers)
	  setConnectedUsersByChannelAndStatus(formattedUsers)
  }, [connectedUsers])

  useEffect(() => {
	if(!availableChannels) return
	if(!connection) return
	let id = 1;
	if(parseInt(ActiveChannelID) !== 0 || typeof ActiveChannelID !== "undefined") id = parseInt(ActiveChannelID)
	if(id === parseInt(channel.id)) return
	connection.send("JoinChannel", availableChannels.find(x=> x.id === id))
  }, [ActiveChannelID, availableChannels])

    return(
      <>
      <div className="container-fluid mh-100">
			<div className="row justify-content-center mh-100">
				<div className="col-md-3 col-xl-3 chat">
					<div className="card mb-sm-3 mb-md-0 contacts_card">
						<div className="card-header">
							<div className="input-group">
								<input type="text" placeholder="Search..." name="" className="form-control search" onChange={e => setSearch(e.target.value)} value={search}/>
								<div className="input-group-prepend">
									<span className="input-group-text search_btn h-100">
										<FontAwesomeIcon icon={faSearch}/>
									</span>
								</div>
							</div>
						</div>
						<div className="card-body contacts_body">
							<ul className="contacts">
								{trimChannels
									? trimChannels.map(channel => 
										<li key={`channel-${channel.id}`} className={parseInt(ActiveChannelID)===parseInt(channel.id) ? "active" : ""} onClick={e => handleChannelSelect(channel)}>
											<div className="d-flex bd-highlight" >
												<div className="img_cont" >
													<img src={channel.image
														? channel.image
														: "https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg"} className="rounded-circle user_img" />
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
										</li>
									)
									: <></>}
							</ul>
						</div>
						<div className="card-footer"></div>
					</div>
				</div>
				<div className="col-md-2 col-xl-2 chat">
					<div className="card mb-sm-3 mb-md-0 contacts_card">
						<div className="card-header msg_head">
							<div className="d-flex bd-highlight">
								<div className="img_cont">
									<img src="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTC9pw-3QlI-doodCy0D-bsxEBZ9lFTcTFsZQ&usqp=CAU" className="rounded-circle user_img"/>
									{isConnectedUsersByChannelAndStatusTruthy() 
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
						<div className="card-footer"></div>
					</div>

				</div>

				<div className="col-md-6 col-xl-6 chat">
					<div className="card">
						<div className="card-header msg_head">
							<div className="d-flex bd-highlight">
								<div className="img_cont">
									<img src={channel.image
												? channel.image
												: "https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg"} className="rounded-circle user_img"/>
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
								<textarea name="" className="form-control type_msg" onKeyDown={e => handleMessageSubmitWithEnterKey(e)} disabled={!user.isPasswordValid} placeholder="Type your message..." onChange={e => handleMessageInput(e)} value={messageText}></textarea>
								<div className="input-group-append">
									{/* <input type="submit" onClick={e => console.log("SUBMIT")}/> */}
									<button className="input-group-text send_btn h-100" onClick={e => handleSendMessage(e)}>
										<FontAwesomeIcon icon={faPaperPlane}
										color={user.isPasswordValid
											? "#90EE90"
											: "#ff4040"}
										/>
									</button>
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