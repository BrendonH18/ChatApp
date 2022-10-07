import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState, useRef } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import { faPaperPlane, faSearch } from "@fortawesome/free-solid-svg-icons"
import axios from 'axios'
import * as methods from "./Methods"


const Chat = ({ jwt, user, channel, availableChannels, messages, connectedUsers, connection, firstChannelId}) => {

const [connectedUsersByChannelAndStatus, setConnectedUsersByChannelAndStatus] = useState(null)
const [messageText, setMessageText] = useState('')
const [search, setSearch] = useState("")
const [trimChannels, setTrimChannels] = useState(availableChannels)

let navigate = useNavigate()
let { ActiveChannelID } = useParams();
const messagesEndRef = useRef(null)

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
	// if (!user.isPasswordValid) return setMessageText("Message blocked - Please login to send messages")
    const message = {
      text: messageText,
      isBot: false
    }
	setMessageText('')
	const authAxios = axios.create({
		baseURL: "https://localhost:44314/",
		headers: {
			Authorization: `Bearer ${jwt}`
		}
	})
	
    authAxios.post("api/chat/sendmessage", message)
	.then(res => console.log(res));
  }

  const scrollToBottom = () => messagesEndRef.current?.scrollIntoView({behavior: "smooth"})

  const trimAvailableChannels = () => {
	  if (search === "") return setTrimChannels(availableChannels)
	  setTrimChannels( availableChannels.filter(ch => ch.name.toLowerCase().includes(search.toLowerCase())))
  }

  
  
  useEffect(scrollToBottom, [messages])

  useEffect(trimAvailableChannels,[search, availableChannels])

  useEffect(() => {
	  const formattedUsers = methods.groupUsers(connectedUsers,availableChannels)
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
								? messages.map(message => { return methods.formatMessage(message, user)})
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

  export default Chat;