import { HubConnection } from "@microsoft/signalr";
import { Outlet, useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { act } from "@testing-library/react";

const ChannelDashboard = ({ availableChannels, messages, connectedUsers, setUserConnection, userConnection, connection, setMessages, setConnectedUsers, isConnectionLoading}) => {
  
const handleClick = (e) => {
  console.table(availableChannels)
  console.log(e.target.id)
  navigate(availableChannels[e.target.id - 1].id.toString())
}

let { ActiveChannelID } = useParams();
// const [activeChannelName, setActiveChannelName] = useState("")

let navigate = useNavigate()
const [activeChannel, setActiveChannel] = useState({id: 1, name: "Loading"})


useEffect(() => {
  if(isConnectionLoading) return
  if(typeof ActiveChannelID === "undefined") return
  let id = parseInt(ActiveChannelID)
  if(id === 0) id = 1
  connection.send("JoinChannel", availableChannels[ id - 1 ])
}, [isConnectionLoading, ActiveChannelID])

const handleChannelSelect = (channel) => {
  navigate(`/${channel.id}`)
}

const formatMessage = (message) => { 
	if (parseInt(message.user.id) === parseInt(userConnection.user.id)) return formatMessageSend(message)
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

const groupBy1 = (objArray, property1,property2) => {
	let newArray = {}
	objArray.forEach(obj => {
		// console.log("1 - Object: ", obj)
		let key = obj[property1][property2]
		// console.log("2 - Key: ", key)
		// console.log("3 - Exists: ", !newArray[key])
		if(!newArray[key]){
			newArray[key] = []
		}
		// console.log("4 - State: ", newArray)
		newArray[key].push(obj)
		// console.log("5 - Final: ", newArray)
	});
	console.log("NEW: ", newArray)
	return newArray
}


	



const countUsersPerChannel = () => {
	// let list = [
	// 	{channel: {id: 1, name: "Sports"}, user: {isPasswordValid: true, username: "Brendon"}, connectionId: "abc"}, 
	// 	{channel: {id: 7, name: "Sports"}, user: {isPasswordValid: false, username: "Claudia"}, connectionId:"xyz"}, 
	// 	{channel: {id: 2, name: "Fashion"}, user: {isPasswordValid: true, username: "Dave"}, connectionId:"azy"}],
	let list = connectedUsers

	let occurences = groupBy1(list, "channel", "id")
	let keys = Object.keys(occurences)
	console.log(keys)
	let newList = []
	for (let i = 0; i < keys.length; i++) {
		// debugger
		const element = occurences[keys[i]];
		// console.log("Key: ", keys[i])
		// console.log("Before: ", newList)
		newList[keys[i]] = []
		// console.log("Middle: ", newList)
		// console.log("Element:", element)
		newList[keys[i]] = groupBy1(element, "user", "isPasswordValid")
		// console.log("End: ", newList)
	}
	console.log("New List: ", newList)
	// groupBy1(occurences,"y")
	// reduce((acc, curr) => {
	// 	return acc[curr] ? ++acc[curr] : acc[curr] = 1, acc
	// }, {})
	// console.log(occurences) 
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
															<span class="online_icon offline" ></span>
										</div>
										<div class="user_info" >
										<span >{channel.name}</span>
										<p >Click Me!</p>
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
									<p>3</p>
									<div>{countUsersPerChannel()}</div>
								</div>
							</div>
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
									<span>{`Let's Chat: ${activeChannel.name}`}</span>
									<p>1767 Messages</p>
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
							<div class="d-flex justify-content-start mb-4">
								<div class="img_cont_msg">
									<img src="https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg" class="rounded-circle user_img_msg"/>
								</div>
								<div class="msg_container">
									Hi, how are you samim?
									<span class="msg_time">8:40 AM, Today</span>
								</div>
							</div>
							<div class="d-flex justify-content-end mb-4">
								<div class="msg_container_send">
									Hi Khalid i am good tnx how about you?
									<span class="msg_time_send">8:55 AM, Today</span>
								</div>
								<div class="img_cont_msg">
							<img src="" class="rounded-circle user_img_msg"/>
								</div>
							</div>
							<div class="d-flex justify-content-start mb-4">
								<div class="img_cont_msg">
									<img src="https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg" class="rounded-circle user_img_msg"/>
								</div>
								<div class="msg_container">
									I am good too
									<span class="msg_time">9:00 AM, Today</span>
								</div>
							</div>
							<div class="d-flex justify-content-end mb-4">
								<div class="msg_container_send">
									You are welcome
									<span class="msg_time_send">9:05 AM, Today</span>
								</div>
								<div class="img_cont_msg">
									<img src="" class="rounded-circle user_img_msg"/>
								</div>
							</div>
							<div class="d-flex justify-content-start mb-4">
								<div class="img_cont_msg">
									<img src="https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg" class="rounded-circle user_img_msg"/>
								</div>
								<div class="msg_container">
									I am looking for your next thing
									<span class="msg_time">9:07 AM, Today</span>
								</div>
							</div>
							<div class="d-flex justify-content-end mb-4">
								<div class="msg_container_send">
									Ok, thank you have a good day
									<span class="msg_time_send">9:10 AM, Today</span>
								</div>
								<div class="img_cont_msg">
						<img src="" class="rounded-circle user_img_msg"/>
								</div>
							</div>
							<div class="d-flex justify-content-start mb-4">
								<div class="img_cont_msg">
									<img src="https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg" class="rounded-circle user_img_msg"/>
								</div>
								<div class="msg_container">
									Bye, see you
									<span class="msg_time">9:12 AM, Today</span>
								</div>
							</div>
							{messages.map(message => { return formatMessage(message)})}
						</div>
						<div class="card-footer">
							<div class="input-group">
								<div class="input-group-append">
									<span class="input-group-text attach_btn"><i class="fas fa-paperclip"></i></span>
								</div>
								<textarea name="" class="form-control type_msg" placeholder="Type your message..."></textarea>
								<div class="input-group-append">
									<span class="input-group-text send_btn"><i class="fas fa-location-arrow"></i></span>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>


      {/* <header className="py-2 mb-4 border-bottom">
        <div className="container">
          <ul className='nav col-12 col-md-auto mb-2 d-flex justify-content-around mb-md-0'>
            {availableChannels.map(x=><button id={x.id} className="px-2 btn btn-primary" onClick={handleClick}>{x.name}</button>)}
          </ul>
        </div>
      </header> */}
      {/* <Outlet/> */}
      </>
    )
  }
  
  export default ChannelDashboard;