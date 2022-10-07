import formatMessageSend from "./formatMessageSend"
import formatMessageReceive from "./formatMessageReceive"

const formatMessage = (message, user) => {
	if (parseInt(message.user.id) === parseInt(user.id)) return formatMessageSend(message)
	return formatMessageReceive(message)
}

export default formatMessage