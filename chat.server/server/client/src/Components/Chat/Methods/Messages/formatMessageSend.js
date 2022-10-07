import formatDateTime from "./formatDateTime"

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

export default formatMessageSend