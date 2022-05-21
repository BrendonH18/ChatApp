import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";

const ChannelDashboard = ({ user, channel, availableChannels, messages, connectedUsers, setUserConnection, userConnection, connection, setMessages, setConnectedUsers, isConnectionLoading}) => {
const blankData = {1:{Active:[], Observe:[]},2:{Active:[], Observe:[]},3:{Active:[], Observe:[]}}

let { ActiveChannelID } = useParams();
const [connectedUsersByChannelAndStatus, setConnectedUsersByChannelAndStatus] = useState(blankData)

let navigate = useNavigate()

const handleChannelSelect = (channel) => {
  navigate(`/Channel/${channel.id}`)
}

useEffect(() => {
  if(isConnectionLoading) return
  if(typeof ActiveChannelID === "undefined") return
  let id = parseInt(ActiveChannelID)
  if(id === 0) id = 1
  connection.send("JoinChannel", availableChannels[ id - 1 ])
}, [isConnectionLoading, ActiveChannelID])


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

useEffect(() => {
	const formattedUsers = groupByChannelId(connectedUsers)
	setConnectedUsersByChannelAndStatus(formattedUsers)
}, [connectedUsers])

const groupByChannelId = (objArray) => {
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
									<span class="input-group-text search_btn"><i class="fas fa-search"></i></span>
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
											{/* <span class={connectedUsersByChannelAndStatus[channel.id]["Active"].length>0 ? "online_icon" : "online_icon offline"} ></span> */}
										</div>
										<div class="user_info" >
										<span >{channel.name}</span>
										{/* <p>{`${connectedUsersByChannelAndStatus[channel.id]["Active"].length} Connected`}</p> */}
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
									<span class="online_icon"></span>
								</div>
								<div class="user_info">
									<span>Users</span>
									{/* <p>{`${connectedUsersByChannelAndStatus[ActiveChannelID]["Active"].length} Connected`}</p> */}
								</div>
							</div>
						</div>
						<div class="card-body contacts_body">
							<ui class="contacts">
								{/* {connectedUsersByChannelAndStatus[ActiveChannelID]["Active"].map(userConnection =>
								<li>
									<div class="d-flex bd-highlight" >
										<div class="user_info" >
											<span >{userConnection.user.username}</span>
										</div>
									</div>
								</li>
								)} */}
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
								<div class="video_cam">
									<span><i class="fas fa-video"></i></span>
									<span><i class="fas fa-phone"></i></span>
								</div>
							</div>
							<span id="action_menu_btn"><i class="fas fa-ellipsis-v"></i></span>
							<div class="action_menu">
								<ul>
									<li><i class="fas fa-user-circle"></i> View profile</li>
									<li><i class="fas fa-users"></i> Add to close friends</li>
									<li><i class="fas fa-plus"></i> Add to group</li>
									<li><i class="fas fa-ban"></i> Block</li>
								</ul>
							</div>
						</div>
						<div class="card-body msg_card_body">
							{messages.map(message => { return formatMessage(message)})}
						</div>
						<div class="card-footer">
							<div class="input-group">
								{/* <div class="input-group-append">
									<span class="input-group-text attach_btn"><i class="fas fa-paperclip"></i></span>
								</div> */}
								<textarea name="" class="form-control type_msg" placeholder="Type your message..."></textarea>
								<div class="input-group-append">
									<span class="input-group-text send_btn"><i class="fa-solid fa-user"></i></span>
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