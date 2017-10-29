package alensia.network

import java.net.InetSocketAddress

import scala.annotation.tailrec

import akka.actor.{ Actor, ActorLogging, ActorRef, PoisonPill }
import akka.io.Udp
import akka.util.ByteString
import alensia.network.BitConverter._
import alensia.network.PacketHandler._

class Peer(
  val address: InetSocketAddress,
  val socket: ActorRef) extends Actor with ActorLogging {

  private var pingSequence: Short = _

  override def preStart() {
    super.preStart()

    log.debug("Created a peer for '{}'.", self.path)
  }

  override def receive: Receive = {
    case data: ByteString =>
      val packet = PacketHandler(data)

      packet match {
        case Merged =>
          log.debug("Processing a merged packet of size: {}.", data.size)

          @tailrec
          def unpack(merged: ByteString): Unit = {
            if (merged.nonEmpty) {
              val size = readShort(merged)

              receive {
                merged.drop(2).take(size)
              }

              unpack(merged.drop(2 + size))
            }
          }

          unpack(data.drop(Merged.headerSize))
        case Ping =>
          //RelativeSequenceNumber(packet.Sequence, _pingSequence) < 0

          pingSequence = Ping.sequence(data)

          log.debug("Received a ping: {}.", pingSequence)

          socket ! Udp.Send(Pong(pingSequence), address)
        case MtuCheck =>
          val mtu = MtuCheck.mtu(data)

          val requested = SupportedMtu(mtu)
          val actual = data.size

          log.debug("Checking MTU: requested = {}, actual = {}.", requested, actual)

          if (requested == actual) {
            socket ! Udp.Send(MtuOk(mtu), address)
          }
        case Unreliable =>
          val payload = Unreliable.payload(data)

          log.info("### Received: {}, {}.", readInt(payload), readInt(payload, 4))
        case Disconnect =>
          self ! PoisonPill
        case _ =>
          log.warning("Unknown packet received: {}.", packet)
      }
  }

  override def postStop() {
    super.postStop()

    log.debug("Stopping the peer for '{}'.", self.path)
  }
}
