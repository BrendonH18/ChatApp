import formatDateTime from "./formatDateTime"

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

export default formatMessageReceive